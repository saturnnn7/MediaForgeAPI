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

                IReadOnlyList<string> outputUrls;
                string? waveformUrl = null;
                if (isVideo)
                {
                    outputUrls = await TranscodeVideoAsync(localFilePath, outputDir, outputBucketName, assetId, ct);
                }
                else
                {
                    var audioResult = await TranscodeAudioAsync(localFilePath, outputDir, outputBucketName, assetId, ct);
                    outputUrls = audioResult.OutputUrls;
                    waveformUrl = audioResult.WaveformUrl;
                }

                var thumbnailKey = $"processed/{assetId}/thumbnail.webp";
                var thumbnailUrl = await storageService.UploadFileAsync(outputBucketName, thumbnailKey, thumbnailWebpPath, "image/webp", ct);

                var (transcriptionText, subtitleUrl) = await TranscribeAsync(localFilePath, outputBucketName, assetId, ct);

                return new ProcessingResult(thumbnailUrl, transcriptionText, outputUrls, durationSeconds, waveformUrl, subtitleUrl);
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

    private async Task<(IReadOnlyList<string> OutputUrls, string? WaveformUrl)> TranscodeAudioAsync(string localFilePath, string outputDir, string outputBucketName, string assetId, CancellationToken ct)
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

        var waveformUrl = await GenerateWaveformAsync(localFilePath, outputDir, outputBucketName, assetId, ct);

        return ([url], waveformUrl);
    }

    private async Task<string> GenerateWaveformAsync(string localFilePath, string outputDir, string outputBucketName, string assetId, CancellationToken ct)
    {
        var pcmPath = Path.Combine(outputDir, "audio.raw");
        await FFMpegArguments
            .FromFileInput(localFilePath)
            .OutputToFile(pcmPath, overwrite: true, opts => opts
                .WithCustomArgument("-f s16le -ac 1 -ar 8000"))
            .ProcessAsynchronously();

        var samples = new short[new FileInfo(pcmPath).Length / 2];
        using (var fs = File.OpenRead(pcmPath))
        using (var br = new BinaryReader(fs))
        {
            for (var i = 0; i < samples.Length; i++)
                samples[i] = br.ReadInt16();
        }

        const int peakCount = 200;
        var peaks = new float[peakCount];
        var samplesPerPeak = Math.Max(1, samples.Length / peakCount);
        for (var i = 0; i < peakCount; i++)
        {
            var start = i * samplesPerPeak;
            var end = Math.Min(start + samplesPerPeak, samples.Length);
            float max = 0;
            for (var j = start; j < end; j++)
                max = Math.Max(max, Math.Abs(samples[j]) / 32768f);
            peaks[i] = max;
        }

        var waveformJson = System.Text.Json.JsonSerializer.Serialize(new { peaks });
        var waveformPath = Path.Combine(outputDir, "waveform.json");
        await File.WriteAllTextAsync(waveformPath, waveformJson, ct);

        var waveformKey = $"processed/{assetId}/waveform.json";
        return await storageService.UploadFileAsync(outputBucketName, waveformKey, waveformPath, "application/json", ct);
    }

    private async Task<(string? Text, string? SubtitleUrl)> TranscribeAsync(
        string filePath, string outputBucketName, string assetId, CancellationToken ct)
    {
        var key = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(key))
        {
            logger.LogWarning("OpenAI API key not configured - skipping transcription.");
            return (null, null);
        }

        try
        {
            var client = openAiClient.GetAudioClient("whisper-1");
            using var fs = File.OpenRead(filePath);
            var options = new AudioTranscriptionOptions
            {
                Language = "en",
                ResponseFormat = AudioTranscriptionFormat.Verbose
            };
            var result = await client.TranscribeAudioAsync(fs, Path.GetFileName(filePath), options, ct);

            if (result.Value.Segments is not { Count: > 0 } segments)
            {
                return (result.Value.Text, null);
            }

            var vttContent = GenerateVtt(segments);
            var vttPath = Path.Combine(Path.GetTempPath(), "mediaforge", assetId, "subtitles.vtt");
            Directory.CreateDirectory(Path.GetDirectoryName(vttPath)!);
            await File.WriteAllTextAsync(vttPath, vttContent, ct);

            var subtitleKey = $"processed/{assetId}/subtitles.vtt";
            var subtitleUrl = await storageService.UploadFileAsync(outputBucketName, subtitleKey, vttPath, "text/vtt", ct);

            return (result.Value.Text, subtitleUrl);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Transcription failed: {Error}", ex.Message);
            return (null, null);
        }
    }

    private static string GenerateVtt(IEnumerable<TranscribedSegment> segments)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("WEBVTT");
        sb.AppendLine();
        var index = 1;
        foreach (var seg in segments)
        {
            sb.AppendLine(index.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.AppendLine(System.Globalization.CultureInfo.InvariantCulture, $"{FormatVttTime(seg.StartTime)} --> {FormatVttTime(seg.EndTime)}");
            sb.AppendLine(seg.Text.Trim());
            sb.AppendLine();
            index++;
        }
        return sb.ToString();
    }

    private static string FormatVttTime(TimeSpan t) =>
        string.Create(System.Globalization.CultureInfo.InvariantCulture, $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}.{t.Milliseconds:D3}");
}
