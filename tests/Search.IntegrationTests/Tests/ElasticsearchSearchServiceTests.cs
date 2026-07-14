using MediaForge.Search.Domain.Models;
using MediaForge.Search.Infrastructure.Search;
using MediaForge.Search.IntegrationTests.Fixtures;

namespace MediaForge.Search.IntegrationTests.Tests;

[Collection("Elasticsearch")]
public sealed class ElasticsearchSearchServiceTests(ElasticsearchFixture fixture)
{
    [Fact]
    public async Task IndexThenSearchReturnsIndexedDocument()
    {
        await ElasticsearchSearchService.EnsureIndexAsync(fixture.Client, CancellationToken.None);
        var service = new ElasticsearchSearchService(fixture.Client);

        var document = new MediaDocument
        {
            Id = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            Title = "Elasticsearch integration test video",
            Description = "A video about testing search",
            TranscriptionText = "This is a transcription mentioning elasticsearch",
            MediaType = "video",
            ThumbnailUrl = "https://example.com/thumb.jpg",
            OutputUrls = ["https://example.com/output.mp4"],
            DurationSeconds = 42,
            CreatedAt = DateTime.UtcNow,
            IndexedAt = DateTime.UtcNow
        };

        await service.IndexAsync(document, CancellationToken.None);
        await fixture.Client.Indices.RefreshAsync();

        var result = await service.SearchAsync("elasticsearch", page: 1, pageSize: 10, CancellationToken.None);

        result.Items.Should().ContainSingle(d => d.Id == document.Id);
        result.TotalCount.Should().Be(1);
    }
}
