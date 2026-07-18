namespace MediaForge.Catalog.Application.DTOs;

public static class SeriesMapper
{
    public static SeriesDto ToDto(this Series series) =>
        new(series.Id, series.ChannelId, series.Title, series.Description, series.CoverUrl, series.IsCompleted, series.CreatedAt);
}
