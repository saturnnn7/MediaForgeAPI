namespace MediaForge.Catalog.Application.Abstractions;

public interface IWorkRepository
{
    Task<Work?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Work?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Work>> GetByChannelIdAsync(Guid channelId, int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyList<Work>> GetBySeriesIdAsync(Guid seriesId, CancellationToken ct);
    Task AddAsync(Work work, CancellationToken ct);
    void Update(Work work);
    Task AddContributorAsync(WorkContributor contributor, CancellationToken ct);
    Task AddGenreAsync(WorkGenre genre, CancellationToken ct);
    Task RemoveContributorAsync(Guid workId, Guid personId, ContributorRole role, CancellationToken ct);
    Task RemoveGenreAsync(Guid workId, Guid genreId, CancellationToken ct);
}
