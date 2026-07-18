namespace MediaForge.Identity.Infrastructure.Persistence.Repositories;

public sealed class ChannelRepository(IdentityDbContext dbContext) : IChannelRepository
{
    public Task<Channel?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Channels.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Channel?> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct) =>
        dbContext.Channels.FirstOrDefaultAsync(c => c.OwnerId == ownerId, ct);

    public Task<Channel?> GetByOwnerIdWithSubscriptionsAsync(Guid ownerId, CancellationToken ct) =>
        dbContext.Channels.FirstOrDefaultAsync(c => c.OwnerId == ownerId, ct);

    public async Task AddAsync(Channel channel, CancellationToken ct) =>
        await dbContext.Channels.AddAsync(channel, ct);

    public void Update(Channel channel) =>
        dbContext.Channels.Update(channel);

    public Task<bool> IsSubscribedAsync(Guid channelId, Guid subscriberId, CancellationToken ct) =>
        dbContext.Subscriptions.AnyAsync(s => s.ChannelId == channelId && s.SubscriberId == subscriberId, ct);

    public async Task AddSubscriptionAsync(ChannelSubscription subscription, CancellationToken ct) =>
        await dbContext.Subscriptions.AddAsync(subscription, ct);

    public async Task RemoveSubscriptionAsync(Guid channelId, Guid subscriberId, CancellationToken ct)
    {
        var subscription = await dbContext.Subscriptions
            .FirstOrDefaultAsync(s => s.ChannelId == channelId && s.SubscriberId == subscriberId, ct);

        if (subscription != null)
        {
            dbContext.Subscriptions.Remove(subscription);
        }
    }

    public async Task<IReadOnlyList<Channel>> GetSubscribedChannelsAsync(Guid subscriberId, CancellationToken ct)
    {
        var channelIds = dbContext.Subscriptions
            .Where(s => s.SubscriberId == subscriberId)
            .Select(s => s.ChannelId);

        return await dbContext.Channels
            .Where(c => channelIds.Contains(c.Id))
            .ToListAsync(ct);
    }
}
