namespace MediaForge.Catalog.Application.Abstractions;

public interface IEditionRepository
{
    Task<Edition?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Edition?> GetByIdWithPartsAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Edition>> GetByWorkIdAsync(Guid workId, CancellationToken ct);
    Task<Edition?> GetDefaultEditionAsync(Guid workId, CancellationToken ct);
    Task AddAsync(Edition edition, CancellationToken ct);
    void Update(Edition edition);
    Task AddPartAsync(Part part, CancellationToken ct);
}
