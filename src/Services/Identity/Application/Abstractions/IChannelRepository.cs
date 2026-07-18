namespace MediaForge.Identity.Application.Abstractions;

public interface IChannelRepository
{
    Task<Channel?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Channel?> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct);
    Task<Channel?> GetByOwnerIdWithSubscriptionsAsync(Guid ownerId, CancellationToken ct);
    Task AddAsync(Channel channel, CancellationToken ct);
    void Update(Channel channel);
    Task<bool> IsSubscribedAsync(Guid channelId, Guid subscriberId, CancellationToken ct);
    Task AddSubscriptionAsync(ChannelSubscription subscription, CancellationToken ct);
    Task RemoveSubscriptionAsync(Guid channelId, Guid subscriberId, CancellationToken ct);
    Task<IReadOnlyList<Channel>> GetSubscribedChannelsAsync(Guid subscriberId, CancellationToken ct);
}
