namespace MediaForge.Identity.Application.DTOs;

public sealed record AdminUserDetailDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    string Role,
    bool IsEmailVerified,
    bool IsBanned,
    string? BanReason,
    DateTime CreatedAt);
