using System.Net.Http.Headers;
using MediaForge.Identity.IntegrationTests.Fixtures;

namespace MediaForge.Identity.IntegrationTests.Tests;

[Collection("Identity")]
public sealed class UserEndpointsTests : IAsyncLifetime, IAsyncDisposable
{
    private readonly IdentityApiFactory _factory;
    private readonly HttpClient _client;

    public UserEndpointsTests(DatabaseFixture fixture)
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
    public async Task GetMeReturns200ForAuthenticatedUser()
    {
        var email = $"me-{Guid.NewGuid():N}@test.local";
        const string password = "Password123";

        await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password, DisplayName = "Me User" });

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokensDto>();

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        var response = await _client.GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<UserProfileDto>();
        body!.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetMeReturns401ForUnauthenticated()
    {
        var response = await _client.GetAsync("/api/users/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
