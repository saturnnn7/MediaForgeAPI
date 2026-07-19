namespace MediaForge.Library.Infrastructure.Persistence.Repositories;

public sealed class ListeningProgressRepository(LibraryDbContext dbContext) : IListeningProgressRepository
{
    public Task<ListeningProgress?> GetAsync(Guid userId, Guid editionId, Guid partId, CancellationToken ct) =>
        dbContext.ListeningProgresses.FirstOrDefaultAsync(
            x => x.UserId == userId && x.EditionId == editionId && x.PartId == partId, ct);

    public async Task<IReadOnlyList<ListeningProgress>> GetByUserAndEditionAsync(Guid userId, Guid editionId, CancellationToken ct) =>
        await dbContext.ListeningProgresses
            .Where(x => x.UserId == userId && x.EditionId == editionId)
            .ToListAsync(ct);

    public async Task AddAsync(ListeningProgress progress, CancellationToken ct) =>
        await dbContext.ListeningProgresses.AddAsync(progress, ct);

    public void Update(ListeningProgress progress) =>
        dbContext.ListeningProgresses.Update(progress);
}
