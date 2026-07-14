using MediaForge.Identity.IntegrationTests.Fixtures;

namespace MediaForge.Identity.IntegrationTests.Tests;

[CollectionDefinition("Identity")]
public sealed class IdentityCollection : ICollectionFixture<DatabaseFixture>;

[Collection("Identity")]
public sealed class AuthEndpointsTests : IAsyncLifetime, IAsyncDisposable
{
    private readonly IdentityApiFactory _factory;
    private readonly HttpClient _client;

    public AuthEndpointsTests(DatabaseFixture fixture)
    {
        _factory = new IdentityApiFactory(fixture);
        _client = _factory.CreateClient();
    }

    public Task InitializeAsync() => _factory.InitializeAsync();

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync() => new(DisposeAsync());

    [Fact]
    public async Task RegisterReturns201WithUserProfile()
    {
        var email = $"user-{Guid.NewGuid():N}@test.local";

        var response = await _client.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = "Password123",
            DisplayName = "Test User"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        body.Should().NotBeNull();
        body!.Email.Should().Be(email);
        body.IsEmailVerified.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterWithDuplicateEmailReturns400()
    {
        var email = $"dup-{Guid.NewGuid():N}@test.local";
        var payload = new { Email = email, Password = "Password123", DisplayName = "Dup User" };

        var first = await _client.PostAsJsonAsync("/api/auth/register", payload);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await _client.PostAsJsonAsync("/api/auth/register", payload);
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task LoginWithValidCredentialsReturns200WithTokens()
    {
        var email = $"login-{Guid.NewGuid():N}@test.local";
        const string password = "Password123";

        await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password, DisplayName = "Login User" });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<TokensDto>();
        body!.AccessToken.Should().NotBeNullOrEmpty();
        body.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginWithWrongPasswordReturns401()
    {
        var email = $"wrongpw-{Guid.NewGuid():N}@test.local";
        const string password = "Password123";

        await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password, DisplayName = "Wrong Password User" });

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = "WrongPassword999" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshTokenReturnsNewTokens()
    {
        var email = $"refresh-{Guid.NewGuid():N}@test.local";
        const string password = "Password123";

        await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password, DisplayName = "Refresh User" });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        var loginTokens = await loginResponse.Content.ReadFromJsonAsync<TokensDto>();

        var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", new { RefreshToken = loginTokens!.RefreshToken });
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshedTokens = await refreshResponse.Content.ReadFromJsonAsync<TokensDto>();
        refreshedTokens!.AccessToken.Should().NotBe(loginTokens.AccessToken);
    }
}
