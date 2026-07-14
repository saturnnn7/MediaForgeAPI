using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Abstractions;

public interface IIdentityGrpcClient
{
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct);
}
