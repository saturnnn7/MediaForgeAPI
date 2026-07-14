namespace MediaForge.Search.Application.Abstractions;

public interface ISearchService
{
    Task IndexAsync(MediaDocument document, CancellationToken ct);
    Task<SearchResultDto> SearchAsync(string query, int page, int pageSize, CancellationToken ct);
    Task DeleteAsync(string documentId, CancellationToken ct);
}
