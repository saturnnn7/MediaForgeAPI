namespace MediaForge.Identity.Application.DTOs;

public sealed record UserProfileDto(
    Guid Id,
    string Email,
    string DisplayName,
    string? AvatarUrl,
    bool IsEmailVerified,
    string Role,
    DateTime CreatedAt);
