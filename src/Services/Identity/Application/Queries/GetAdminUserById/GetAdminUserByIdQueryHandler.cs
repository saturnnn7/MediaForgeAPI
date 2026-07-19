using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetAdminUserById;

public sealed class GetAdminUserByIdQueryHandler(
    IApplicationUserRepository userRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetAdminUserByIdQuery, Result<AdminUserDetailDto>>
{
    public async Task<Result<AdminUserDetailDto>> Handle(GetAdminUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure<AdminUserDetailDto>(Error.Unauthorized("Only an admin can view user details."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<AdminUserDetailDto>(Error.NotFound("User", request.UserId));
        }

        return Result.Success(new AdminUserDetailDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.AvatarUrl,
            user.Role.ToString().ToLowerInvariant(),
            user.IsEmailVerified,
            user.IsBanned,
            user.BanReason,
            user.CreatedAt));
    }
}
