namespace MediaForge.Search.Application.Abstractions;

public interface ISearchService
{
    Task IndexAsync(MediaDocument document, CancellationToken ct);
    Task<SearchResultDto<MediaDocument>> SearchAsync(string query, int page, int pageSize, CancellationToken ct);
    Task DeleteAsync(string documentId, CancellationToken ct);

    Task IndexWorkAsync(WorkDocument document, CancellationToken ct);
    Task<SearchResultDto<WorkDocument>> SearchWorksAsync(string query, int page, int pageSize, CancellationToken ct);
    Task DeleteWorkAsync(string workId, CancellationToken ct);

    Task IndexPersonAsync(PersonDocument document, CancellationToken ct);
    Task<SearchResultDto<PersonDocument>> SearchPersonsAsync(string query, int page, int pageSize, CancellationToken ct);
}
