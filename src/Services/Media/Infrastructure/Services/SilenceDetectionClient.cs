using System.Globalization;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Infrastructure.Services;

public sealed class SilenceDetectionClient(
    IStorageService storageService,
    ISilenceDetectionService silenceDetectionService) : IMediaProcessingClient
{
    private const double MinSilenceDurationSeconds = 1.0;

    public async Task<IReadOnlyList<SuggestedChapterDto>> GetSuggestedChaptersAsync(string bucketName, string objectKey, CancellationToken ct)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "mediaforge", "silence-detection");
        Directory.CreateDirectory(tempDir);
        var tempPath = Path.Combine(tempDir, string.Create(CultureInfo.InvariantCulture, $"{Guid.NewGuid()}{Path.GetExtension(objectKey)}"));

        try
        {
            await storageService.DownloadToFileAsync(bucketName, objectKey, tempPath, ct);

            var silences = await silenceDetectionService.DetectSilenceAsync(tempPath, ct);

            var boundaries = silences
                .Where(s => s.EndSeconds - s.StartSeconds > MinSilenceDurationSeconds)
                .Select(s => s.MidpointSeconds)
                .OrderBy(s => s)
                .ToList();

            var suggestions = new List<SuggestedChapterDto> { new(1, 0, "Chapter 1") };

            for (var i = 0; i < boundaries.Count; i++)
            {
                var order = i + 2;
                suggestions.Add(new SuggestedChapterDto(order, boundaries[i], string.Create(CultureInfo.InvariantCulture, $"Chapter {order}")));
            }

            return suggestions;
        }
        finally
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
        }
    }
}
