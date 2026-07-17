using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetUserProfile;

public sealed class GetUserProfileQueryHandler(
    ICurrentUserService currentUser,
    IApplicationUserRepository userRepository) : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<UserProfileDto>(Error.NotFound("User", currentUser.UserId));
        }

        return Result.Success(new UserProfileDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.AvatarUrl,
            user.IsEmailVerified,
            user.Role.ToString().ToLowerInvariant(),
            user.CreatedAt));
    }
}
