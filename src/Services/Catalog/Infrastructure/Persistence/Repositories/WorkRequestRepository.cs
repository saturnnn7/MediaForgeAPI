namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class WorkRequestRepository(CatalogDbContext dbContext) : IWorkRequestRepository
{
    public Task<WorkRequest?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.WorkRequests.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<WorkRequest>> GetByRequesterIdAsync(Guid requesterId, CancellationToken ct) =>
        await dbContext.WorkRequests
            .Where(x => x.RequesterId == requesterId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<WorkRequest>> GetPendingAsync(CancellationToken ct) =>
        await dbContext.WorkRequests
            .Where(x => x.Status == WorkRequestStatus.Pending)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(WorkRequest request, CancellationToken ct) =>
        await dbContext.WorkRequests.AddAsync(request, ct);

    public void Update(WorkRequest request) =>
        dbContext.WorkRequests.Update(request);
}
