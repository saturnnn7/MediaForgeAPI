using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetAdminUsers;

public sealed class GetAdminUsersQueryHandler(
    IApplicationUserRepository userRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetAdminUsersQuery, Result<AdminUserListDto>>
{
    public async Task<Result<AdminUserListDto>> Handle(GetAdminUsersQuery request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure<AdminUserListDto>(Error.Unauthorized("Only an admin can list users."));
        }

        var users = await userRepository.GetPagedAsync(request.Page, request.PageSize, request.Role, request.Search, cancellationToken);
        var total = await userRepository.CountAsync(request.Role, request.Search, cancellationToken);

        var dto = new AdminUserListDto(
            users.Select(u => new AdminUserSummaryDto(
                u.Id,
                u.Email,
                u.DisplayName,
                u.Role.ToString().ToLowerInvariant(),
                u.IsEmailVerified,
                u.IsBanned,
                u.CreatedAt)).ToList(),
            total,
            request.Page,
            request.PageSize);

        return Result.Success(dto);
    }
}
