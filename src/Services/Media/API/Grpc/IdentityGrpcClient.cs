using Grpc.Core;
using MediaForge.Identity.API.Grpc;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace MediaForge.Media.API.Grpc;

public sealed class IdentityGrpcClient(UserService.UserServiceClient client, ILogger<IdentityGrpcClient> logger) : IIdentityGrpcClient
{
    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct)
    {
        try
        {
            var request = new GetUserByIdRequest { UserId = userId.ToString() };
            var response = await client.GetUserByIdAsync(request, cancellationToken: ct);
            if (!response.Found)
                return null;

            return new UserDto(Guid.Parse(response.UserId), response.Email, response.DisplayName, response.IsEmailVerified);
        }
        catch (RpcException ex)
        {
            logger.LogWarning("gRPC call to Identity failed: {Status}", ex.Status);
            return null;
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("BrokenCircuit")
                                    || ex.GetType().Name.Contains("Timeout"))
        {
            logger.LogWarning("Identity service circuit breaker open or timeout: {Message}", ex.Message);
            return null;
        }
    }
}
