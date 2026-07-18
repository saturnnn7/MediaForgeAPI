namespace MediaForge.Catalog.Application.DTOs;

public sealed record SeriesDto(
    Guid Id,
    Guid ChannelId,
    string Title,
    string? Description,
    string? CoverUrl,
    bool IsCompleted,
    DateTime CreatedAt);
