namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class WorkRepository(CatalogDbContext dbContext) : IWorkRepository
{
    public Task<Work?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Works.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Work?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct) =>
        dbContext.Works
            .Include("_contributors")
            .Include("_genres")
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Work>> GetByChannelIdAsync(Guid channelId, int page, int pageSize, CancellationToken ct) =>
        await dbContext.Works
            .Where(x => x.ChannelId == channelId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Work>> GetBySeriesIdAsync(Guid seriesId, CancellationToken ct) =>
        await dbContext.Works
            .Where(x => x.SeriesId == seriesId)
            .ToListAsync(ct);

    public async Task AddAsync(Work work, CancellationToken ct) =>
        await dbContext.Works.AddAsync(work, ct);

    public void Update(Work work) =>
        dbContext.Works.Update(work);

    public async Task AddContributorAsync(WorkContributor contributor, CancellationToken ct) =>
        await dbContext.WorkContributors.AddAsync(contributor, ct);

    public async Task AddGenreAsync(WorkGenre genre, CancellationToken ct) =>
        await dbContext.WorkGenres.AddAsync(genre, ct);

    public async Task RemoveContributorAsync(Guid workId, Guid personId, ContributorRole role, CancellationToken ct)
    {
        var contributor = await dbContext.WorkContributors
            .FirstOrDefaultAsync(x => x.WorkId == workId && x.PersonId == personId && x.Role == role, ct);

        if (contributor is not null)
            dbContext.WorkContributors.Remove(contributor);
    }

    public async Task RemoveGenreAsync(Guid workId, Guid genreId, CancellationToken ct)
    {
        var genre = await dbContext.WorkGenres
            .FirstOrDefaultAsync(x => x.WorkId == workId && x.GenreId == genreId, ct);

        if (genre is not null)
            dbContext.WorkGenres.Remove(genre);
    }

    public async Task<IReadOnlyList<(WorkContributor Contributor, Person Person)>> GetContributorsWithPersonsAsync(Guid workId, CancellationToken ct)
    {
        var rows = await dbContext.WorkContributors
            .Where(c => c.WorkId == workId)
            .Join(dbContext.Persons, c => c.PersonId, p => p.Id, (c, p) => new { Contributor = c, Person = p })
            .ToListAsync(ct);

        return rows.Select(x => (x.Contributor, x.Person)).ToList();
    }
}
