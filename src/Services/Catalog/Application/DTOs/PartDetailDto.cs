namespace MediaForge.Catalog.Application.DTOs;

public sealed record PartDetailDto(
    Guid Id,
    Guid WorkId,
    string Title,
    string? Description,
    int OrderMajor,
    int OrderMinor,
    string PartType,
    string? CoverUrl,
    bool IsPublished,
    double? DurationSeconds,
    DateTime CreatedAt,
    IReadOnlyList<PartChapterDto> Chapters,
    IReadOnlyList<PartAssetDto> Assets);
