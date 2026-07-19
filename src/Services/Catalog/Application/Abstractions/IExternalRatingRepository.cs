namespace MediaForge.Catalog.Application.Abstractions;

public interface IExternalRatingRepository
{
    Task<ExternalRating?> GetByWorkAndSourceAsync(Guid workId, ExternalRatingSource source, CancellationToken ct);
    Task<IReadOnlyList<ExternalRating>> GetByWorkIdAsync(Guid workId, CancellationToken ct);
    Task AddAsync(ExternalRating rating, CancellationToken ct);
    void Update(ExternalRating rating);
}
