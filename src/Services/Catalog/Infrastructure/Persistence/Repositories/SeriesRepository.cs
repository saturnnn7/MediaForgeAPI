namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class SeriesRepository(CatalogDbContext dbContext) : ISeriesRepository
{
    public Task<Series?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Series.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Series>> GetByChannelIdAsync(Guid channelId, int page, int pageSize, CancellationToken ct) =>
        await dbContext.Series
            .Where(x => x.ChannelId == channelId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(Series series, CancellationToken ct) =>
        await dbContext.Series.AddAsync(series, ct);

    public void Update(Series series) =>
        dbContext.Series.Update(series);
}
