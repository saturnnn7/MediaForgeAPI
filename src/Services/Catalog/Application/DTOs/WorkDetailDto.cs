namespace MediaForge.Catalog.Application.DTOs;

public sealed record WorkDetailDto(
    Guid Id,
    Guid ChannelId,
    Guid? SeriesId,
    string WorkType,
    string Title,
    string? Description,
    string? CoverUrl,
    string? Language,
    bool IsPublished,
    bool IsPrivate,
    DateTime? PublishedAt,
    DateTime CreatedAt,
    IReadOnlyList<ContributorDto> Contributors,
    IReadOnlyList<GenreDto> Genres,
    IReadOnlyList<ExternalRatingDto> ExternalRatings);
