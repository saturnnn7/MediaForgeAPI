using MediaForge.Gateway.YARP.Hubs;

namespace MediaForge.Gateway.YARP.Consumers;

public sealed class MediaProcessingCompletedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<MediaProcessingCompletedEvent>
{
    public async Task Consume(ConsumeContext<MediaProcessingCompletedEvent> context)
    {
        var groupName = $"user:{context.Message.UserId}";

        await hubContext.Clients.Group(groupName).SendAsync(
            "MediaProcessingCompleted",
            new
            {
                assetId = context.Message.AssetId,
                thumbnailUrl = context.Message.ThumbnailUrl,
                outputUrls = context.Message.OutputUrls,
                transcriptionText = context.Message.TranscriptionText,
                durationSeconds = context.Message.DurationSeconds
            },
            context.CancellationToken);
    }
}
