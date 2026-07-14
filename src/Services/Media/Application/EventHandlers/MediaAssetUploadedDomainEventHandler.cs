using Microsoft.Extensions.Logging;

namespace MediaForge.Media.Application.EventHandlers;

public sealed class MediaAssetUploadedDomainEventHandler(ILogger<MediaAssetUploadedDomainEventHandler> logger)
    : INotificationHandler<MediaAssetUploadedDomainEvent>
{
    public Task Handle(MediaAssetUploadedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Media asset {AssetId} uploaded by user {UserId}, queued for processing.",
            notification.AssetId,
            notification.UserId);

        return Task.CompletedTask;
    }
}
