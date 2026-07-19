using MassTransit;
using MediaForge.Shared.Contracts.Events.Identity;
using Microsoft.Extensions.Logging;

namespace MediaForge.Catalog.Infrastructure.Consumers;

public sealed class UserUnsubscribedConsumer(
    ISubscriptionService subscriptionService,
    ILogger<UserUnsubscribedConsumer> logger) : IConsumer<UserUnsubscribedFromChannelEvent>
{
    public async Task Consume(ConsumeContext<UserUnsubscribedFromChannelEvent> context)
    {
        var msg = context.Message;
        await subscriptionService.RemoveSubscriptionAsync(msg.ChannelId, msg.SubscriberId, context.CancellationToken);
        logger.LogInformation("User {SubscriberId} unsubscribed from channel {ChannelId}", msg.SubscriberId, msg.ChannelId);
    }
}
