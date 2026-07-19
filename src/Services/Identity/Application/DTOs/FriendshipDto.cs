namespace MediaForge.Identity.Application.DTOs;

public sealed record FriendshipDto(
    Guid Id,
    Guid RequesterId,
    Guid AddresseeId,
    string Status,
    DateTime CreatedAt,
    DateTime? RespondedAt);
