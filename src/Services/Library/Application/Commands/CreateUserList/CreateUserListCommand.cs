using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.CreateUserList;

public sealed record CreateUserListCommand(
    string Name,
    string Slug,
    string? Description,
    string? AvatarUrl,
    ListPrivacy Privacy) : IRequest<Result<UserListDto>>;
