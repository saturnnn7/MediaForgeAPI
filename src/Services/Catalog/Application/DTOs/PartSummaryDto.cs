namespace MediaForge.Catalog.Application.DTOs;

public sealed record PartSummaryDto(
    Guid Id,
    Guid WorkId,
    string Title,
    int OrderMajor,
    int OrderMinor,
    string PartType,
    bool IsPublished,
    double? DurationSeconds);
