namespace MediaForge.Catalog.Application.DTOs;

public static class WorkMapper
{
    public static WorkSummaryDto ToSummaryDto(this Work work) =>
        new(
            work.Id,
            work.ChannelId,
            work.SeriesId,
            work.WorkType.ToString(),
            work.Title,
            work.CoverUrl,
            work.Language,
            work.IsPublished,
            work.IsPrivate,
            work.CreatedAt);

    public static WorkDetailDto ToDetailDto(
        this Work work,
        IReadOnlyList<ContributorDto> contributors,
        IReadOnlyList<GenreDto> genres) =>
        new(
            work.Id,
            work.ChannelId,
            work.SeriesId,
            work.WorkType.ToString(),
            work.Title,
            work.Description,
            work.CoverUrl,
            work.Language,
            work.IsPublished,
            work.IsPrivate,
            work.PublishedAt,
            work.CreatedAt,
            contributors,
            genres);
}
