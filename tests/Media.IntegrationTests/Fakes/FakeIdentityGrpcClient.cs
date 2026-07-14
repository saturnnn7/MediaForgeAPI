namespace MediaForge.Media.IntegrationTests.Fakes;

public sealed class FakeIdentityGrpcClient : IIdentityGrpcClient
{
    public Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct) =>
        Task.FromResult<UserDto?>(new UserDto(userId, "test@test.local", "Test User", IsEmailVerified: true));
}
