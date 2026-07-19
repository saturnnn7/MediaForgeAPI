using MediaForge.Library.Domain.Enums;
using MediaForge.Library.IntegrationTests.Fixtures;

namespace MediaForge.Library.IntegrationTests.Tests;

[Collection("Library")]
public sealed class LibraryEndpointsTests : IAsyncLifetime, IAsyncDisposable
{
    private readonly LibraryApiFactory _factory;
    private readonly HttpClient _client;

    public LibraryEndpointsTests(LibraryInfrastructureFixture fixture)
    {
        _factory = new LibraryApiFactory(fixture);
        _client = _factory.CreateAuthenticatedClient(Guid.NewGuid());
    }

    public Task InitializeAsync() => _factory.InitializeAsync();

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync() => new(DisposeAsync());

    [Fact]
    public async Task AddToLibraryReturns201WithValidWorkId()
    {
        var response = await AddToLibrary(_client, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<LibraryEntryDto>();
        body!.Status.Should().Be("Planned");
    }

    [Fact]
    public async Task AddToLibraryReturns409WhenDuplicate()
    {
        var workId = Guid.NewGuid();

        var first = await AddToLibrary(_client, workId);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await AddToLibrary(_client, workId);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateLibraryStatusReturns200()
    {
        var workId = Guid.NewGuid();
        await AddToLibrary(_client, workId);

        var response = await _client.PutAsJsonAsync($"/api/library/{workId}/status", new { NewStatus = LibraryStatus.Watching });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ToggleFavoriteReturns200()
    {
        var workId = Guid.NewGuid();
        await AddToLibrary(_client, workId);

        var first = await _client.PostAsync($"/api/library/{workId}/favorite", null);
        first.StatusCode.Should().Be(HttpStatusCode.OK);

        var second = await _client.PostAsync($"/api/library/{workId}/favorite", null);
        second.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RateWorkReturns200WithValidRating()
    {
        var workId = Guid.NewGuid();
        await AddToLibrary(_client, workId);

        var response = await _client.PutAsJsonAsync($"/api/library/{workId}/rating", new { Rating = 8 });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RateWorkReturns400WithInvalidRating()
    {
        var workId = Guid.NewGuid();
        await AddToLibrary(_client, workId);

        var response = await _client.PutAsJsonAsync($"/api/library/{workId}/rating", new { Rating = 11 });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReviewReturns201()
    {
        var response = await CreateReview(_client, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateReviewReturns409WhenDuplicate()
    {
        var workId = Guid.NewGuid();

        var first = await CreateReview(_client, workId);
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await CreateReview(_client, workId);
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddCommentReturns201OnExistingReview()
    {
        var reviewResponse = await CreateReview(_client, Guid.NewGuid());
        var review = await reviewResponse.Content.ReadFromJsonAsync<ReviewDto>();

        var response = await _client.PostAsJsonAsync($"/api/reviews/{review!.Id}/comments", new
        {
            ParentCommentId = (Guid?)null,
            Text = "Great review!",
            ContainsSpoiler = false
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetWorkReviewsReturns200WithReviews()
    {
        var workId = Guid.NewGuid();
        await CreateReview(_client, workId);

        var response = await _client.GetAsync($"/api/works/{workId}/reviews");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUserListReturns201()
    {
        var response = await CreateUserList(_client);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task AddWorkToListReturns200()
    {
        var listResponse = await CreateUserList(_client);
        var list = await listResponse.Content.ReadFromJsonAsync<UserListDto>();

        var response = await _client.PostAsJsonAsync($"/api/lists/{list!.Id}/works", new
        {
            WorkId = Guid.NewGuid(),
            DisplayOrder = 1
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static Task<HttpResponseMessage> AddToLibrary(HttpClient client, Guid workId) =>
        client.PostAsJsonAsync("/api/library", new
        {
            WorkId = workId,
            Status = LibraryStatus.Planned,
            Privacy = ListPrivacy.Everyone
        });

    private static Task<HttpResponseMessage> CreateReview(HttpClient client, Guid workId) =>
        client.PostAsJsonAsync("/api/reviews", new
        {
            WorkId = workId,
            Text = "A great listen.",
            ContainsSpoiler = false
        });

    private static Task<HttpResponseMessage> CreateUserList(HttpClient client) =>
        client.PostAsJsonAsync("/api/lists", new
        {
            Name = "My Favorites",
            Slug = $"my-favorites-{Guid.NewGuid():N}",
            Privacy = ListPrivacy.Everyone
        });
}
