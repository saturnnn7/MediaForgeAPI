using MassTransit;
using MediaForge.Shared.Contracts.Events.Identity;
using Microsoft.Extensions.Logging;

namespace MediaForge.Catalog.Infrastructure.Consumers;

public sealed class UserSubscribedConsumer(
    ISubscriptionService subscriptionService,
    ILogger<UserSubscribedConsumer> logger) : IConsumer<UserSubscribedToChannelEvent>
{
    public async Task Consume(ConsumeContext<UserSubscribedToChannelEvent> context)
    {
        var msg = context.Message;
        await subscriptionService.AddSubscriptionAsync(msg.ChannelId, msg.SubscriberId, context.CancellationToken);
        logger.LogInformation("User {SubscriberId} subscribed to channel {ChannelId}", msg.SubscriberId, msg.ChannelId);
    }
}
