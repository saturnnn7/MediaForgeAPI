using MediaForge.Catalog.Domain.Enums;
using MediaForge.Catalog.IntegrationTests.Fixtures;

namespace MediaForge.Catalog.IntegrationTests.Tests;

[Collection("Catalog")]
public sealed class CatalogEndpointsTests : IAsyncLifetime, IAsyncDisposable
{
    private readonly CatalogApiFactory _factory;
    private readonly HttpClient _adminClient;
    private readonly HttpClient _creatorClient;

    public CatalogEndpointsTests(CatalogInfrastructureFixture fixture)
    {
        _factory = new CatalogApiFactory(fixture);
        _adminClient = _factory.CreateAuthenticatedClient(Guid.NewGuid(), "admin");
        _creatorClient = _factory.CreateAuthenticatedClient(Guid.NewGuid(), "creator");
    }

    public Task InitializeAsync() => _factory.InitializeAsync();

    public async Task DisposeAsync()
    {
        _adminClient.Dispose();
        _creatorClient.Dispose();
        await _factory.DisposeAsync();
    }

    ValueTask IAsyncDisposable.DisposeAsync() => new(DisposeAsync());

    [Fact]
    public async Task CreateGenreReturns201WithValidData()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/genres", new
        {
            Name = "Fantasy",
            Slug = $"fantasy-{Guid.NewGuid():N}",
            Description = "Fantasy genre"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<GenreDto>();
        body!.Id.Should().NotBeEmpty();
        body.Name.Should().Be("Fantasy");
    }

    [Fact]
    public async Task CreateGenreReturns409WhenSlugDuplicated()
    {
        var slug = $"sci-fi-{Guid.NewGuid():N}";

        var first = await _adminClient.PostAsJsonAsync("/api/genres", new { Name = "Sci-Fi", Slug = slug, Description = (string?)null });
        first.StatusCode.Should().Be(HttpStatusCode.Created);

        var second = await _adminClient.PostAsJsonAsync("/api/genres", new { Name = "Sci-Fi Again", Slug = slug, Description = (string?)null });
        second.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreatePersonReturns201WithValidData()
    {
        var response = await _adminClient.PostAsJsonAsync("/api/persons", new
        {
            Name = "Jane Narrator",
            Bio = "A narrator",
            PhotoUrl = (string?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<PersonDto>();
        body!.Name.Should().Be("Jane Narrator");
    }

    [Fact]
    public async Task SubmitWorkRequestReturns201ForCreator()
    {
        var response = await SubmitWorkRequest(_creatorClient);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<WorkRequestDto>();
        body!.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task ApproveWorkRequestCreatesWork()
    {
        var submitResponse = await SubmitWorkRequest(_creatorClient);
        var workRequest = await submitResponse.Content.ReadFromJsonAsync<WorkRequestDto>();

        var approveResponse = await _adminClient.PostAsJsonAsync(
            $"/api/work-requests/{workRequest!.Id}/approve",
            new { ChannelId = Guid.NewGuid() });

        approveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var approved = await approveResponse.Content.ReadFromJsonAsync<WorkRequestDto>();
        approved!.ResultingWorkId.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateEditionReturns201ForApprovedWork()
    {
        var workId = await CreateApprovedWork();

        var response = await CreateEdition(workId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<EditionDto>();
        body!.IsDefault.Should().BeTrue();
    }

    [Fact]
    public async Task GetWorkEditionsReturns200WithEditions()
    {
        var workId = await CreateApprovedWork();
        await CreateEdition(workId);

        var response = await _creatorClient.GetAsync($"/api/works/{workId}/editions");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<List<EditionDto>>();
        body!.Count.Should().BeGreaterOrEqualTo(1);
    }

    [Fact]
    public async Task CreatePartReturns201ForEdition()
    {
        var workId = await CreateApprovedWork();
        var editionResponse = await CreateEdition(workId);
        var edition = await editionResponse.Content.ReadFromJsonAsync<EditionDto>();

        var response = await _creatorClient.PostAsJsonAsync("/api/parts", new
        {
            EditionId = edition!.Id,
            Title = "Chapter One",
            Description = (string?)null,
            OrderMajor = 1,
            OrderMinor = 0,
            PartType = PartType.Regular,
            CoverUrl = (string?)null
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<PartSummaryDto>();
        body!.Title.Should().Be("Chapter One");
        body.OrderMajor.Should().Be(1);
    }

    [Fact]
    public async Task PublishWorkReturns200()
    {
        var workId = await CreateApprovedWork();

        var publishResponse = await _creatorClient.PostAsync($"/api/works/{workId}/publish", null);
        publishResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _creatorClient.GetAsync($"/api/works/{workId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var work = await getResponse.Content.ReadFromJsonAsync<WorkDetailDto>();
        work!.IsPublished.Should().BeTrue();
    }

    private static async Task<HttpResponseMessage> SubmitWorkRequest(HttpClient client) =>
        await client.PostAsJsonAsync("/api/work-requests", new
        {
            WorkType = WorkType.Audiobook,
            Title = $"Test Work {Guid.NewGuid():N}",
            AuthorNames = "Some Author",
            Description = (string?)null,
            CoverUrl = (string?)null,
            Language = (string?)null
        });

    private async Task<Guid> CreateApprovedWork()
    {
        var submitResponse = await SubmitWorkRequest(_creatorClient);
        var workRequest = await submitResponse.Content.ReadFromJsonAsync<WorkRequestDto>();

        var approveResponse = await _adminClient.PostAsJsonAsync(
            $"/api/work-requests/{workRequest!.Id}/approve",
            new { ChannelId = Guid.NewGuid() });

        var approved = await approveResponse.Content.ReadFromJsonAsync<WorkRequestDto>();
        return approved!.ResultingWorkId!.Value;
    }

    private async Task<HttpResponseMessage> CreateEdition(Guid workId) =>
        await _creatorClient.PostAsJsonAsync("/api/editions", new
        {
            WorkId = workId,
            NarratorTeamName = "The Narrators",
            Description = (string?)null,
            CoverUrl = (string?)null,
            Language = (string?)null
        });
}
