using FFMpegCore;
using FFMpegCore.Enums;
using MediaForge.Processing.Worker.Models;
using OpenAI;
using OpenAI.Audio;

namespace MediaForge.Processing.Worker.Services;

public sealed class MediaProcessingService(
    IStorageService storageService,
    IConfiguration configuration,
    OpenAIClient openAiClient,
    ILogger<MediaProcessingService> logger) : IMediaProcessingService
{
    private static readonly SemaphoreSlim ProcessingSemaphore = new(1, 1);

    private static readonly (int Height, int Bitrate)[] Qualities =
    [
        (1080, 5000),
        (720, 2800),
        (480, 1400)
    ];

    public async Task<ProcessingResult> ProcessAsync(
        string localFilePath,
        string contentType,
        string originalFileName,
        string outputBucketName,
        string assetId,
        CancellationToken ct)
    {
        var isVideo = contentType.StartsWith("video/", StringComparison.Ordinal);
        var outputDir = Path.Combine(Path.GetTempPath(), "mediaforge", assetId);
        Directory.CreateDirectory(outputDir);

        try
        {
            await ProcessingSemaphore.WaitAsync(ct);
            try
            {
                var mediaInfo = await FFProbe.AnalyseAsync(localFilePath, cancellationToken: ct);
                var durationSeconds = mediaInfo.Duration.TotalSeconds;

                var thumbnailWebpPath = await GenerateThumbnailAsync(localFilePath, outputDir, isVideo, ct);

                var outputUrls = isVideo
                    ? await TranscodeVideoAsync(localFilePath, outputDir, outputBucketName, assetId, ct)
                    : await TranscodeAudioAsync(localFilePath, outputDir, outputBucketName, assetId, ct);

                var thumbnailKey = $"processed/{assetId}/thumbnail.webp";
                var thumbnailUrl = await storageService.UploadFileAsync(outputBucketName, thumbnailKey, thumbnailWebpPath, "image/webp", ct);

                var transcriptionText = await TranscribeAsync(localFilePath, ct);

                return new ProcessingResult(thumbnailUrl, transcriptionText, outputUrls, durationSeconds);
            }
            finally
            {
                ProcessingSemaphore.Release();
            }
        }
        finally
        {
            if (Directory.Exists(outputDir))
            {
                Directory.Delete(outputDir, recursive: true);
            }
        }
    }

    private static async Task<string> GenerateThumbnailAsync(string localFilePath, string outputDir, bool isVideo, CancellationToken ct)
    {
        var thumbnailPath = Path.Combine(outputDir, "thumbnail.jpg");

        if (isVideo)
        {
            await FFMpegArguments
                .FromFileInput(localFilePath, verifyExists: true, opts => opts.Seek(TimeSpan.FromSeconds(1)))
                .OutputToFile(thumbnailPath, overwrite: true, opts => opts
                    .WithVideoFilters(f => f.Scale(1280, -1))
                    .WithFrameOutputCount(1))
                .ProcessAsynchronously();
        }
        else
        {
            await FFMpegArguments
                .FromFileInput(localFilePath)
                .OutputToFile(thumbnailPath, overwrite: true, opts => opts
                    .WithCustomArgument("-filter_complex showwavespic=s=1280x200:colors=white")
                    .WithFrameOutputCount(1))
                .ProcessAsynchronously();
        }

        var thumbnailWebpPath = Path.Combine(outputDir, "thumbnail.webp");
        await FFMpegArguments
            .FromFileInput(thumbnailPath)
            .OutputToFile(thumbnailWebpPath, overwrite: true, opts => opts.ForceFormat("webp"))
            .ProcessAsynchronously();

        return thumbnailWebpPath;
    }

    private async Task<IReadOnlyList<string>> TranscodeVideoAsync(string localFilePath, string outputDir, string outputBucketName, string assetId, CancellationToken ct)
    {
        var outputUrls = new List<string>();
        var hlsDir = Path.Combine(outputDir, "hls");
        Directory.CreateDirectory(hlsDir);

        foreach (var quality in Qualities)
        {
            var variantDir = Path.Combine(hlsDir, $"{quality.Height}p");
            Directory.CreateDirectory(variantDir);
            var outputPath = Path.Combine(variantDir, "stream.m3u8");

            await FFMpegArguments
                .FromFileInput(localFilePath)
                .OutputToFile(outputPath, overwrite: true, opts => opts
                    .WithVideoCodec("libx264")
                    .WithAudioCodec("aac")
                    .WithCustomArgument($"-vf scale=-2:{quality.Height}")
                    .WithCustomArgument($"-b:v {quality.Bitrate}k")
                    .WithCustomArgument("-hls_time 6")
                    .WithCustomArgument("-hls_playlist_type vod")
                    .WithCustomArgument($"-hls_segment_filename {variantDir}/segment%03d.ts"))
                .ProcessAsynchronously();

            foreach (var file in Directory.GetFiles(variantDir))
            {
                var ext = Path.GetExtension(file) == ".ts" ? "video/MP2T" : "application/vnd.apple.mpegurl";
                var key = $"processed/{assetId}/hls/{quality.Height}p/{Path.GetFileName(file)}";
                var url = await storageService.UploadFileAsync(outputBucketName, key, file, ext, ct);

                if (file.EndsWith(".m3u8", StringComparison.Ordinal))
                {
                    outputUrls.Add(url);
                }
            }
        }

        return outputUrls;
    }

    private async Task<IReadOnlyList<string>> TranscodeAudioAsync(string localFilePath, string outputDir, string outputBucketName, string assetId, CancellationToken ct)
    {
        var mp3Path = Path.Combine(outputDir, "output.mp3");

        await FFMpegArguments
            .FromFileInput(localFilePath)
            .OutputToFile(mp3Path, overwrite: true, opts => opts
                .WithAudioCodec("libmp3lame")
                .WithAudioBitrate(192))
            .ProcessAsynchronously();

        var key = $"processed/{assetId}/audio.mp3";
        var url = await storageService.UploadFileAsync(outputBucketName, key, mp3Path, "audio/mpeg", ct);

        return [url];
    }

    private async Task<string?> TranscribeAsync(string filePath, CancellationToken ct)
    {
        var key = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(key))
        {
            logger.LogWarning("OpenAI API key not configured — skipping transcription.");
            return null;
        }

        try
        {
            var client = openAiClient.GetAudioClient("whisper-1");
            using var fs = File.OpenRead(filePath);
            var result = await client.TranscribeAudioAsync(fs, Path.GetFileName(filePath),
                new AudioTranscriptionOptions { Language = "en" }, ct);
            return result.Value.Text;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Transcription failed: {Error}", ex.Message);
            return null;
        }
    }
}
