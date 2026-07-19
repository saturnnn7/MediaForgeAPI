namespace MediaForge.Identity.Application.DTOs;

public sealed record NotificationDto(
    Guid Id,
    Guid UserId,
    string Type,
    string Payload,
    bool IsRead,
    DateTime CreatedAt);
