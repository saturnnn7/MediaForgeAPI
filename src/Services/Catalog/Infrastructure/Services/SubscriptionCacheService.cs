using Microsoft.Extensions.Caching.Distributed;

namespace MediaForge.Catalog.Infrastructure.Services;

public sealed class SubscriptionCacheService(IDistributedCache cache) : ISubscriptionService
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
    };

    public async Task<bool> IsSubscribedAsync(Guid channelId, Guid userId, CancellationToken ct)
    {
        var value = await cache.GetStringAsync(BuildKey(channelId, userId), ct);
        return value is not null;
    }

    public Task AddSubscriptionAsync(Guid channelId, Guid userId, CancellationToken ct) =>
        cache.SetStringAsync(BuildKey(channelId, userId), "1", CacheOptions, ct);

    public Task RemoveSubscriptionAsync(Guid channelId, Guid userId, CancellationToken ct) =>
        cache.RemoveAsync(BuildKey(channelId, userId), ct);

    private static string BuildKey(Guid channelId, Guid userId) => $"subscription:{channelId}:{userId}";
}
