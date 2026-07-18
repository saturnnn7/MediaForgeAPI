namespace MediaForge.Catalog.Application.DTOs;

public static class PartMapper
{
    public static PartSummaryDto ToSummaryDto(this Part part) =>
        new(
            part.Id,
            part.WorkId,
            part.Title,
            part.OrderMajor,
            part.OrderMinor,
            part.PartType.ToString(),
            part.IsPublished,
            part.DurationSeconds);

    public static PartDetailDto ToDetailDto(this Part part) =>
        new(
            part.Id,
            part.WorkId,
            part.Title,
            part.Description,
            part.OrderMajor,
            part.OrderMinor,
            part.PartType.ToString(),
            part.CoverUrl,
            part.IsPublished,
            part.DurationSeconds,
            part.CreatedAt,
            part.Chapters.Select(c => c.ToDto()).ToList(),
            part.Assets.Select(a => a.ToDto()).ToList());

    public static PartChapterDto ToDto(this PartChapter chapter) =>
        new(chapter.Id, chapter.Title, chapter.StartTimeSeconds, chapter.EndTimeSeconds, chapter.Order);

    public static PartAssetDto ToDto(this PartAsset asset) =>
        new(asset.Id, asset.MediaAssetId, asset.SequenceOrder);
}
