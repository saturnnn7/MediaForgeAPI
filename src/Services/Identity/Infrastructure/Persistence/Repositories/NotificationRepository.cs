namespace MediaForge.Identity.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(IdentityDbContext dbContext) : INotificationRepository
{
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct) =>
        await dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct) =>
        dbContext.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct);

    public Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task AddAsync(Notification notification, CancellationToken ct) =>
        await dbContext.Notifications.AddAsync(notification, ct);

    public async Task MarkAllReadAsync(Guid userId, CancellationToken ct)
    {
        await dbContext.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
    }

    public void Update(Notification notification) =>
        dbContext.Notifications.Update(notification);
}
