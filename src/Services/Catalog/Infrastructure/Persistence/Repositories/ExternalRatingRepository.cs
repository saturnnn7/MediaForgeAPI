namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class ExternalRatingRepository(CatalogDbContext dbContext) : IExternalRatingRepository
{
    public Task<ExternalRating?> GetByWorkAndSourceAsync(Guid workId, ExternalRatingSource source, CancellationToken ct) =>
        dbContext.ExternalRatings.FirstOrDefaultAsync(x => x.WorkId == workId && x.Source == source, ct);

    public async Task<IReadOnlyList<ExternalRating>> GetByWorkIdAsync(Guid workId, CancellationToken ct) =>
        await dbContext.ExternalRatings
            .Where(x => x.WorkId == workId)
            .ToListAsync(ct);

    public async Task AddAsync(ExternalRating rating, CancellationToken ct) =>
        await dbContext.ExternalRatings.AddAsync(rating, ct);

    public void Update(ExternalRating rating) =>
        dbContext.ExternalRatings.Update(rating);
}
