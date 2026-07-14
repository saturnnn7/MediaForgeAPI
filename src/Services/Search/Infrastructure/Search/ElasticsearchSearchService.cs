using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;
using Elastic.Clients.Elasticsearch.QueryDsl;
using MediaForge.Search.Application.Abstractions;
using MediaForge.Search.Application.DTOs;
using MediaForge.Search.Domain.Models;

namespace MediaForge.Search.Infrastructure.Search;

public sealed class ElasticsearchSearchService(ElasticsearchClient client) : ISearchService
{
    public const string IndexName = "media_documents";

    private static readonly string[] SearchFields = ["title", "transcriptionText", "description"];

    public static async Task EnsureIndexAsync(ElasticsearchClient client, CancellationToken ct)
    {
        var exists = await client.Indices.ExistsAsync(IndexName, ct);
        if (exists.Exists)
        {
            return;
        }

        await client.Indices.CreateAsync(IndexName, c => c
            .Mappings(m => m
                .Properties(new Properties
                {
                    { "title", new TextProperty() },
                    { "transcriptionText", new TextProperty() },
                    { "description", new TextProperty() },
                    { "userId", new KeywordProperty() },
                    { "mediaType", new KeywordProperty() },
                    { "indexedAt", new DateProperty() }
                })), ct);
    }

    public async Task IndexAsync(MediaDocument document, CancellationToken ct)
    {
        await client.IndexAsync(document, idx => idx.Index(IndexName), ct);
    }

    public async Task<SearchResultDto> SearchAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var response = await client.SearchAsync<MediaDocument>(s => s
            .Index(IndexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(SearchFields)
                    .Query(query)
                    .Type(TextQueryType.BestFields)
                    .Fuzziness(new Fuzziness("AUTO")))), ct);

        var totalCount = response.HitsMetadata?.Total?.Match(totalHits => totalHits.Value, longValue => longValue) ?? 0;

        return new SearchResultDto(
            response.Documents.ToList().AsReadOnly(),
            totalCount,
            page,
            pageSize);
    }

    public async Task DeleteAsync(string documentId, CancellationToken ct)
    {
        await client.DeleteAsync(new DeleteRequest(IndexName, documentId), ct);
    }
}
