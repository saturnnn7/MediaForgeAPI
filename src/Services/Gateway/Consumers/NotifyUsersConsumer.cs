using MediaForge.Gateway.YARP.Hubs;
using MediaForge.Shared.Contracts.Events.Library;

namespace MediaForge.Gateway.YARP.Consumers;

public sealed class NotifyUsersConsumer(
    IHubContext<NotificationHub> hubContext,
    ILogger<NotifyUsersConsumer> logger) : IConsumer<NotifyUsersEvent>
{
    public async Task Consume(ConsumeContext<NotifyUsersEvent> context)
    {
        var message = context.Message;

        foreach (var userId in message.UserIds)
        {
            var groupName = $"user:{userId}";

            await hubContext.Clients.Group(groupName).SendAsync(
                message.NotificationType,
                new
                {
                    payload = message.Payload,
                    occurredOn = message.OccurredOn
                },
                context.CancellationToken);
        }

        logger.LogInformation(
            "Pushed {NotificationType} notification to {UserCount} users",
            message.NotificationType,
            message.UserIds.Count);
    }
}
