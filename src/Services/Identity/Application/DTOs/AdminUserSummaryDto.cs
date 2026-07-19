namespace MediaForge.Identity.Application.DTOs;

public sealed record AdminUserSummaryDto(
    Guid Id,
    string Email,
    string DisplayName,
    string Role,
    bool IsEmailVerified,
    bool IsBanned,
    DateTime CreatedAt);
