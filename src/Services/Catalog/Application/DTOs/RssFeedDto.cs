namespace MediaForge.Catalog.Application.DTOs;

public sealed record RssFeedDto(
    string ChannelTitle,
    string? ChannelDescription,
    string? ChannelImageUrl,
    string ChannelLink,
    DateTime LastBuildDate,
    IReadOnlyList<RssFeedItemDto> Items);

public sealed record RssFeedItemDto(
    string Title,
    string? Description,
    string ItemGuid,
    DateTime PublishedAt,
    string? AudioUrl,
    double? DurationSeconds,
    int OrderMajor,
    int OrderMinor);
