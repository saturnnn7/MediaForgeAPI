namespace MediaForge.Media.Application.DTOs;

internal static class MediaAssetMapper
{
    public static MediaAssetDto ToDto(this MediaAsset asset) =>
        new(
            asset.Id,
            asset.UserId,
            asset.FileName,
            asset.ContentType,
            asset.FileSizeBytes,
            asset.MediaType.ToString(),
            asset.Status.ToString(),
            asset.ThumbnailUrl,
            asset.TranscriptionText,
            asset.OutputUrls,
            asset.DurationSeconds,
            asset.CreatedAt,
            asset.ProcessingCompletedAt);
}
