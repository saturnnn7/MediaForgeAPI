namespace MediaForge.Catalog.Application.DTOs;

public sealed record PartSummaryDto(
    Guid Id,
    Guid EditionId,
    string? NarratorTeamName,
    string Title,
    int OrderMajor,
    int OrderMinor,
    string PartType,
    bool IsPublished,
    double? DurationSeconds);
