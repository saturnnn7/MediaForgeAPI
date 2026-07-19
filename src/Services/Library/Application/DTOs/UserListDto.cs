namespace MediaForge.Library.Application.DTOs;

public sealed record UserListDto(
    Guid Id,
    Guid UserId,
    string Name,
    string Slug,
    string? Description,
    string? AvatarUrl,
    string Privacy,
    bool IsSystem,
    int ItemCount,
    DateTime CreatedAt);
