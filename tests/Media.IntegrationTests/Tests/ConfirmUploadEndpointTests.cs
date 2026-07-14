using MediaForge.Media.IntegrationTests.Fixtures;

namespace MediaForge.Media.IntegrationTests.Tests;

[Collection("Media")]
public sealed class ConfirmUploadEndpointTests : IAsyncLifetime, IAsyncDisposable
{
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly Guid OtherUserId = Guid.NewGuid();

    private readonly MediaApiFactory _factory;
    private readonly HttpClient _client;

    public ConfirmUploadEndpointTests(MediaInfrastructureFixture fixture)
    {
        _factory = new MediaApiFactory(fixture);
        _client = _factory.CreateAuthenticatedClient(TestUserId, "test@test.local");
    }

    public Task InitializeAsync() => _factory.InitializeAsync();

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync() => new(DisposeAsync());

    private static async Task<Guid> CreateAssetAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/media/upload-url", new
        {
            FileName = "test.mp4",
            ContentType = "video/mp4",
            FileSizeBytes = 1_048_576
        });

        var body = await response.Content.ReadFromJsonAsync<UploadUrlDto>();
        return body!.AssetId;
    }

    [Fact]
    public async Task ConfirmUploadReturns202ForOwnAsset()
    {
        var assetId = await CreateAssetAsync(_client);

        var response = await _client.PostAsync($"/api/media/{assetId}/confirm-upload", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task ConfirmUploadReturns404ForNonExistentAsset()
    {
        var response = await _client.PostAsync($"/api/media/{Guid.NewGuid()}/confirm-upload", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ConfirmUploadReturns401ForDifferentUser()
    {
        var assetId = await CreateAssetAsync(_client);

        using var otherClient = _factory.CreateAuthenticatedClient(OtherUserId, "other@test.local");
        var response = await otherClient.PostAsync($"/api/media/{assetId}/confirm-upload", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ConfirmUploadIsIdempotentOnlyOnce()
    {
        var assetId = await CreateAssetAsync(_client);

        var first = await _client.PostAsync($"/api/media/{assetId}/confirm-upload", content: null);
        first.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var second = await _client.PostAsync($"/api/media/{assetId}/confirm-upload", content: null);
        second.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
