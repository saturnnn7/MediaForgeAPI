using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.UpdateUserList;

public sealed record UpdateUserListCommand(
    Guid ListId,
    string Name,
    string? Description,
    string? AvatarUrl,
    ListPrivacy Privacy) : IRequest<Result<UserListDto>>;
