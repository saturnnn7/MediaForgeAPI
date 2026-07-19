namespace MediaForge.Catalog.Application.Abstractions;

public interface ISubscriptionService
{
    Task<bool> IsSubscribedAsync(Guid channelId, Guid userId, CancellationToken ct);
    Task AddSubscriptionAsync(Guid channelId, Guid userId, CancellationToken ct);
    Task RemoveSubscriptionAsync(Guid channelId, Guid userId, CancellationToken ct);
}
