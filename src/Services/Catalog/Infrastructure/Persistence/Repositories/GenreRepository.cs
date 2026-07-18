namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class GenreRepository(CatalogDbContext dbContext) : IGenreRepository
{
    public Task<Genre?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Genres.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Genre?> GetBySlugAsync(string slug, CancellationToken ct) =>
        dbContext.Genres.FirstOrDefaultAsync(x => x.Slug == slug, ct);

    public async Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken ct) =>
        await dbContext.Genres
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

    public async Task AddAsync(Genre genre, CancellationToken ct) =>
        await dbContext.Genres.AddAsync(genre, ct);

    public void Update(Genre genre) =>
        dbContext.Genres.Update(genre);
}
