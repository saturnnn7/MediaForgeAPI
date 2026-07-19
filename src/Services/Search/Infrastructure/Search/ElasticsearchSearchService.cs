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
    public const string WorksIndexName = "works";
    public const string PersonsIndexName = "persons";

    private static readonly string[] SearchFields = ["title", "transcriptionText", "description"];
    private static readonly string[] WorkSearchFields = ["title", "description", "contributorNames"];
    private static readonly string[] PersonSearchFields = ["name", "bio"];

    public static async Task EnsureIndexAsync(ElasticsearchClient client, CancellationToken ct)
    {
        var exists = await client.Indices.ExistsAsync(IndexName, ct);
        if (!exists.Exists)
        {
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

        var worksExist = await client.Indices.ExistsAsync(WorksIndexName, ct);
        if (!worksExist.Exists)
        {
            await client.Indices.CreateAsync(WorksIndexName, c => c
                .Mappings(m => m
                    .Properties(new Properties
                    {
                        { "title", new TextProperty() },
                        { "description", new TextProperty() },
                        { "workType", new KeywordProperty() },
                        { "language", new KeywordProperty() },
                        { "contributorNames", new TextProperty() },
                        { "genreNames", new KeywordProperty() },
                        { "channelId", new KeywordProperty() },
                        { "indexedAt", new DateProperty() }
                    })), ct);
        }

        var personsExist = await client.Indices.ExistsAsync(PersonsIndexName, ct);
        if (!personsExist.Exists)
        {
            await client.Indices.CreateAsync(PersonsIndexName, c => c
                .Mappings(m => m
                    .Properties(new Properties
                    {
                        { "name", new TextProperty() },
                        { "bio", new TextProperty() },
                        { "indexedAt", new DateProperty() }
                    })), ct);
        }
    }

    public async Task IndexAsync(MediaDocument document, CancellationToken ct)
    {
        await client.IndexAsync(document, idx => idx.Index(IndexName), ct);
    }

    public async Task<SearchResultDto<MediaDocument>> SearchAsync(string query, int page, int pageSize, CancellationToken ct)
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

        if (!response.IsValidResponse || response.HitsMetadata is null)
            return new SearchResultDto<MediaDocument>(Array.Empty<MediaDocument>(), 0, page, pageSize);

        var totalCount = response.HitsMetadata.Total?.Match(totalHits => totalHits.Value, longValue => longValue) ?? 0;

        return new SearchResultDto<MediaDocument>(
            response.Documents.ToList().AsReadOnly(),
            totalCount,
            page,
            pageSize);
    }

    public async Task DeleteAsync(string documentId, CancellationToken ct)
    {
        await client.DeleteAsync(new DeleteRequest(IndexName, documentId), ct);
    }

    public async Task IndexWorkAsync(WorkDocument document, CancellationToken ct)
    {
        await client.IndexAsync(document, idx => idx.Index(WorksIndexName).Id(document.Id), ct);
    }

    public async Task<SearchResultDto<WorkDocument>> SearchWorksAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var response = await client.SearchAsync<WorkDocument>(s => s
            .Index(WorksIndexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(WorkSearchFields)
                    .Query(query)
                    .Type(TextQueryType.BestFields)
                    .Fuzziness(new Fuzziness("AUTO")))), ct);

        if (!response.IsValidResponse || response.HitsMetadata is null)
            return new SearchResultDto<WorkDocument>(Array.Empty<WorkDocument>(), 0, page, pageSize);

        var totalCount = response.HitsMetadata.Total?.Match(totalHits => totalHits.Value, longValue => longValue) ?? 0;

        return new SearchResultDto<WorkDocument>(
            response.Documents.ToList().AsReadOnly(),
            totalCount,
            page,
            pageSize);
    }

    public async Task DeleteWorkAsync(string workId, CancellationToken ct)
    {
        await client.DeleteAsync(new DeleteRequest(WorksIndexName, workId), ct);
    }

    public async Task IndexPersonAsync(PersonDocument document, CancellationToken ct)
    {
        await client.IndexAsync(document, idx => idx.Index(PersonsIndexName).Id(document.Id), ct);
    }

    public async Task<SearchResultDto<PersonDocument>> SearchPersonsAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var response = await client.SearchAsync<PersonDocument>(s => s
            .Index(PersonsIndexName)
            .From((page - 1) * pageSize)
            .Size(pageSize)
            .Query(q => q
                .MultiMatch(mm => mm
                    .Fields(PersonSearchFields)
                    .Query(query)
                    .Type(TextQueryType.BestFields)
                    .Fuzziness(new Fuzziness("AUTO")))), ct);

        if (!response.IsValidResponse || response.HitsMetadata is null)
            return new SearchResultDto<PersonDocument>(Array.Empty<PersonDocument>(), 0, page, pageSize);

        var totalCount = response.HitsMetadata.Total?.Match(totalHits => totalHits.Value, longValue => longValue) ?? 0;

        return new SearchResultDto<PersonDocument>(
            response.Documents.ToList().AsReadOnly(),
            totalCount,
            page,
            pageSize);
    }
}
