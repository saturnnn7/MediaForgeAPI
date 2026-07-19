namespace MediaForge.Catalog.Application.Abstractions;

public interface IWorkRequestRepository
{
    Task<WorkRequest?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<WorkRequest>> GetByRequesterIdAsync(Guid requesterId, CancellationToken ct);
    Task<IReadOnlyList<WorkRequest>> GetPendingAsync(CancellationToken ct);
    Task<IReadOnlyList<WorkRequest>> GetAllAsync(WorkRequestStatus? status, int page, int pageSize, CancellationToken ct);
    Task AddAsync(WorkRequest request, CancellationToken ct);
    void Update(WorkRequest request);
}
