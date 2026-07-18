namespace MediaForge.Catalog.Application.DTOs;

public sealed record WorkSummaryDto(
    Guid Id,
    Guid ChannelId,
    Guid? SeriesId,
    string WorkType,
    string Title,
    string? CoverUrl,
    string? Language,
    bool IsPublished,
    DateTime CreatedAt);
