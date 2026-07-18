namespace MediaForge.Catalog.Application.Abstractions;

public interface IGenreRepository
{
    Task<Genre?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Genre?> GetBySlugAsync(string slug, CancellationToken ct);
    Task<IReadOnlyList<Genre>> GetAllAsync(CancellationToken ct);
    Task AddAsync(Genre genre, CancellationToken ct);
    void Update(Genre genre);
}
