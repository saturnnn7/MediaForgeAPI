namespace MediaForge.Library.Application.Abstractions;

public interface IListeningProgressRepository
{
    Task<ListeningProgress?> GetAsync(Guid userId, Guid editionId, Guid partId, CancellationToken ct);
    Task<IReadOnlyList<ListeningProgress>> GetByUserAndEditionAsync(Guid userId, Guid editionId, CancellationToken ct);
    Task AddAsync(ListeningProgress progress, CancellationToken ct);
    void Update(ListeningProgress progress);
}
