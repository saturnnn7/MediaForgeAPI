namespace MediaForge.Identity.Application.DTOs;

public sealed record AuthorFollowDto(
    Guid Id,
    Guid UserId,
    Guid PersonId,
    DateTime FollowedAt);
