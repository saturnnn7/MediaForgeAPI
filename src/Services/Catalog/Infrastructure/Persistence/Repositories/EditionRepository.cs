namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class EditionRepository(CatalogDbContext dbContext) : IEditionRepository
{
    public Task<Edition?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Editions.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Edition?> GetByIdWithPartsAsync(Guid id, CancellationToken ct) =>
        dbContext.Editions
            .Include("_parts")
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Edition>> GetByWorkIdAsync(Guid workId, CancellationToken ct) =>
        await dbContext.Editions
            .Include("_parts")
            .Where(x => x.WorkId == workId)
            .OrderByDescending(x => x.IsDefault).ThenBy(x => x.CreatedAt)
            .ToListAsync(ct);

    public Task<Edition?> GetDefaultEditionAsync(Guid workId, CancellationToken ct) =>
        dbContext.Editions.FirstOrDefaultAsync(x => x.WorkId == workId && x.IsDefault, ct);

    public async Task AddAsync(Edition edition, CancellationToken ct) =>
        await dbContext.Editions.AddAsync(edition, ct);

    public void Update(Edition edition) =>
        dbContext.Editions.Update(edition);

    public async Task AddPartAsync(Part part, CancellationToken ct) =>
        await dbContext.Parts.AddAsync(part, ct);
}
