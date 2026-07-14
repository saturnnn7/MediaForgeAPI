using MediaForge.Search.Application.Abstractions;
using MediaForge.Search.Application.DTOs;
using MediaForge.Search.Domain.Models;

namespace MediaForge.Search.Infrastructure.Search;

public sealed class InMemorySearchService : ISearchService
{
    private static readonly List<MediaDocument> Store = [];

    public Task IndexAsync(MediaDocument document, CancellationToken ct)
    {
        Store.Add(document);
        return Task.CompletedTask;
    }

    public Task<SearchResultDto> SearchAsync(string query, int page, int pageSize, CancellationToken ct)
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

        return Task.FromResult(new SearchResultDto(items, matches.Count, page, pageSize));
    }

    public Task DeleteAsync(string documentId, CancellationToken ct)
    {
        Store.RemoveAll(d => d.Id == documentId);
        return Task.CompletedTask;
    }
}
