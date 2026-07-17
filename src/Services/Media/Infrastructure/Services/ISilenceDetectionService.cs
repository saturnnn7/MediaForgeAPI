using MediaForge.Media.Infrastructure.Models;

namespace MediaForge.Media.Infrastructure.Services;

public interface ISilenceDetectionService
{
    Task<IReadOnlyList<SilenceSegment>> DetectSilenceAsync(string filePath, CancellationToken ct);
}
