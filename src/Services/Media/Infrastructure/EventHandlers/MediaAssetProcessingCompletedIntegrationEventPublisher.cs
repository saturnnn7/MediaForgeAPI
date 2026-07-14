using MassTransit;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Domain.Events;
using MediaForge.Shared.Contracts.Events.Media;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MediaForge.Media.Infrastructure.EventHandlers;

public sealed class MediaAssetProcessingCompletedIntegrationEventPublisher(
    IPublishEndpoint publishEndpoint,
    IMediaAssetRepository mediaAssetRepository,
    ILogger<MediaAssetProcessingCompletedIntegrationEventPublisher> logger)
    : INotificationHandler<MediaAssetProcessingCompletedDomainEvent>
{
    public async Task Handle(MediaAssetProcessingCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(notification.AssetId, cancellationToken);
        if (asset is null)
        {
            logger.LogWarning("Media asset {AssetId} not found when publishing processing-completed event.", notification.AssetId);
            return;
        }

        await publishEndpoint.Publish(
            new MediaProcessingCompletedEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                Guid.NewGuid(),
                asset.Id,
                asset.UserId,
                asset.TranscriptionText,
                asset.OutputUrls,
                asset.ThumbnailUrl ?? string.Empty,
                asset.DurationSeconds ?? 0),
            cancellationToken);
    }
}
