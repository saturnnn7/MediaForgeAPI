namespace MediaForge.Media.Application.DTOs;

public sealed record StreamingUrlsDto(
    Guid AssetId,
    string ThumbnailUrl,
    IReadOnlyList<StreamingVariantDto> Variants,
    string? SubtitleUrl,
    double DurationSeconds,
    string? WaveformUrl,
    IReadOnlyList<ChapterDto> Chapters,
    IReadOnlyList<string> OutputUrls,
    Guid? PartId);

public sealed record StreamingVariantDto(
    int Height,
    string PlaylistUrl,
    int BitrateKbps);
