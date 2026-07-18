namespace MediaForge.Identity.Application.DTOs;

public sealed record ChannelDto(
    Guid Id,
    string Name,
    string? Description,
    string? AvatarUrl,
    int SubscriberCount,
    DateTime CreatedAt,
    bool IsSubscribed);
