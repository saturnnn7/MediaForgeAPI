namespace MediaForge.Library.Application.DTOs;

public sealed record UserListItemDto(
    Guid Id,
    Guid WorkId,
    int DisplayOrder,
    DateTime AddedAt);
