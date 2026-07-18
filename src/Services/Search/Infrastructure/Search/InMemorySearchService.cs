using MediaForge.Search.Application.Abstractions;
using MediaForge.Search.Application.DTOs;
using MediaForge.Search.Domain.Models;

namespace MediaForge.Search.Infrastructure.Search;

public sealed class InMemorySearchService : ISearchService
{
    private static readonly List<MediaDocument> Store = [];
    private static readonly List<WorkDocument> WorksStore = [];
    private static readonly List<PersonDocument> PersonsStore = [];

    public Task IndexAsync(MediaDocument document, CancellationToken ct)
    {
        Store.Add(document);
        return Task.CompletedTask;
    }

    public Task<SearchResultDto<MediaDocument>> SearchAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var matches = Store
            .Where(d =>
                d.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (d.TranscriptionText?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        var items = matches
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new SearchResultDto<MediaDocument>(items, matches.Count, page, pageSize));
    }

    public Task DeleteAsync(string documentId, CancellationToken ct)
    {
        Store.RemoveAll(d => d.Id == documentId);
        return Task.CompletedTask;
    }

    public Task IndexWorkAsync(WorkDocument document, CancellationToken ct)
    {
        WorksStore.RemoveAll(w => w.Id == document.Id);
        WorksStore.Add(document);
        return Task.CompletedTask;
    }

    public Task<SearchResultDto<WorkDocument>> SearchWorksAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var matches = WorksStore
            .Where(w =>
                w.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (w.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                w.ContributorNames.Any(n => n.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var items = matches
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new SearchResultDto<WorkDocument>(items, matches.Count, page, pageSize));
    }

    public Task DeleteWorkAsync(string workId, CancellationToken ct)
    {
        WorksStore.RemoveAll(w => w.Id == workId);
        return Task.CompletedTask;
    }

    public Task IndexPersonAsync(PersonDocument document, CancellationToken ct)
    {
        PersonsStore.RemoveAll(p => p.Id == document.Id);
        PersonsStore.Add(document);
        return Task.CompletedTask;
    }

    public Task<SearchResultDto<PersonDocument>> SearchPersonsAsync(string query, int page, int pageSize, CancellationToken ct)
    {
        var matches = PersonsStore
            .Where(p =>
                p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (p.Bio?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        var items = matches
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new SearchResultDto<PersonDocument>(items, matches.Count, page, pageSize));
    }
}
