using Grpc.Core;
using MediaForge.Identity.Domain.Entities;

namespace MediaForge.Identity.API.Grpc;

public sealed class UserGrpcService(IApplicationUserRepository userRepository) : UserService.UserServiceBase
{
    public override async Task<UserResponse> GetUserById(GetUserByIdRequest request, ServerCallContext context)
    {
        var user = await userRepository.GetByIdAsync(Guid.Parse(request.UserId), context.CancellationToken);
        return Map(user);
    }

    public override async Task<UserResponse> GetUserByEmail(GetUserByEmailRequest request, ServerCallContext context)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, context.CancellationToken);
        return Map(user);
    }

    private static UserResponse Map(ApplicationUser? user)
    {
        if (user is null)
        {
            return new UserResponse { Found = false };
        }

        return new UserResponse
        {
            UserId = user.Id.ToString(),
            Email = user.Email,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl ?? string.Empty,
            IsEmailVerified = user.IsEmailVerified,
            Found = true
        };
    }
}
