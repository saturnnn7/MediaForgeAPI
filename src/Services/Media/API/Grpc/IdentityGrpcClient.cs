using MediaForge.Identity.API.Grpc;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.API.Grpc;

public sealed class IdentityGrpcClient(UserService.UserServiceClient client) : IIdentityGrpcClient
{
    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct)
    {
        var response = await client.GetUserByIdAsync(
            new GetUserByIdRequest { UserId = userId.ToString() },
            cancellationToken: ct);

        if (!response.Found)
            return null;

        return new UserDto(Guid.Parse(response.UserId), response.Email, response.DisplayName, response.IsEmailVerified);
    }
}
