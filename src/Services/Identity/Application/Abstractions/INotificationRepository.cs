namespace MediaForge.Identity.Application.Abstractions;

public interface INotificationRepository
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct);
    Task AddAsync(Notification notification, CancellationToken ct);
    Task MarkAllReadAsync(Guid userId, CancellationToken ct);
    void Update(Notification notification);
}
