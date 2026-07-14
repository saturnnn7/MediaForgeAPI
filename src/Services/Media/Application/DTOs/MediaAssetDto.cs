namespace MediaForge.Media.Application.DTOs;

public sealed record MediaAssetDto(
    Guid Id,
    Guid UserId,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string MediaType,
    string Status,
    string? ThumbnailUrl,
    string? TranscriptionText,
    IReadOnlyList<string> OutputUrls,
    double? DurationSeconds,
    DateTime CreatedAt,
    DateTime? ProcessingCompletedAt);
