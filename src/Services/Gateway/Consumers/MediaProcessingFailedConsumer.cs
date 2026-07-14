using MediaForge.Gateway.YARP.Hubs;

namespace MediaForge.Gateway.YARP.Consumers;

public sealed class MediaProcessingFailedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<MediaProcessingFailedEvent>
{
    public async Task Consume(ConsumeContext<MediaProcessingFailedEvent> context)
    {
        var groupName = $"user:{context.Message.UserId}";

        await hubContext.Clients.Group(groupName).SendAsync(
            "MediaProcessingFailed",
            new
            {
                assetId = context.Message.AssetId,
                reason = context.Message.Reason
            },
            context.CancellationToken);
    }
}
