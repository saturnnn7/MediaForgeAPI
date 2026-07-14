namespace MediaForge.Processing.Worker.Models;

public sealed record ProcessingResult(
    string ThumbnailUrl,
    string? TranscriptionText,
    IReadOnlyList<string> OutputUrls,
    double DurationSeconds);
