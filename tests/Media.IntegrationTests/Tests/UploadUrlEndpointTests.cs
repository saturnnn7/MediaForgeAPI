using MediaForge.Media.IntegrationTests.Fixtures;

namespace MediaForge.Media.IntegrationTests.Tests;

[Collection("Media")]
public sealed class UploadUrlEndpointTests : IAsyncLifetime, IAsyncDisposable
{
    private static readonly Guid TestUserId = Guid.NewGuid();

    private readonly MediaApiFactory _factory;
    private readonly HttpClient _client;

    public UploadUrlEndpointTests(MediaInfrastructureFixture fixture)
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

    [Fact]
    public async Task RequestUploadUrlReturns201ForValidVideo()
    {
        var response = await _client.PostAsJsonAsync("/api/media/upload-url", new
        {
            FileName = "test.mp4",
            ContentType = "video/mp4",
            FileSizeBytes = 1_048_576
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<UploadUrlDto>();
        body!.AssetId.Should().NotBeEmpty();
        body.UploadUrl.Should().Contain("minioadmin");
    }

    [Fact]
    public async Task RequestUploadUrlReturns201ForValidAudio()
    {
        var response = await _client.PostAsJsonAsync("/api/media/upload-url", new
        {
            FileName = "podcast.mp3",
            ContentType = "audio/mpeg",
            FileSizeBytes = 1_048_576
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<UploadUrlDto>();
        body!.AssetId.Should().NotBeEmpty();
        body.UploadUrl.Should().Contain("minioadmin");
    }

    [Fact]
    public async Task RequestUploadUrlReturns400ForUnsupportedContentType()
    {
        var response = await _client.PostAsJsonAsync("/api/media/upload-url", new
        {
            FileName = "image.png",
            ContentType = "image/png",
            FileSizeBytes = 1_048_576
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RequestUploadUrlReturns400ForExceededFileSize()
    {
        var response = await _client.PostAsJsonAsync("/api/media/upload-url", new
        {
            FileName = "huge.mp4",
            ContentType = "video/mp4",
            FileSizeBytes = 3_000_000_000
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RequestUploadUrlReturns401ForUnauthenticated()
    {
        using var anonymousClient = _factory.CreateClient();

        var response = await anonymousClient.PostAsJsonAsync("/api/media/upload-url", new
        {
            FileName = "test.mp4",
            ContentType = "video/mp4",
            FileSizeBytes = 1_048_576
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
