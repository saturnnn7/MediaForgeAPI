namespace MediaForge.Identity.Domain.Entities;

public sealed class ChannelSubscription
{
    private ChannelSubscription() { }

    public Guid Id { get; init; }
    public Guid ChannelId { get; init; }
    public Guid SubscriberId { get; init; }
    public DateTime SubscribedAt { get; init; }

    public static ChannelSubscription Create(Guid channelId, Guid subscriberId) =>
        new()
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            SubscriberId = subscriberId,
            SubscribedAt = DateTime.UtcNow
        };
}
