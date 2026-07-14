using MassTransit;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Shared.Contracts.Events.Media;
using Microsoft.Extensions.Logging;

namespace MediaForge.Media.Infrastructure.Consumers;

public sealed class MediaProcessingCompletedConsumer(
    IMediaAssetRepository repository,
    IMediaUnitOfWork unitOfWork,
    ILogger<MediaProcessingCompletedConsumer> logger) : IConsumer<MediaProcessingCompletedEvent>
{
    public async Task Consume(ConsumeContext<MediaProcessingCompletedEvent> context)
    {
        var msg = context.Message;
        var asset = await repository.GetByIdAsync(msg.AssetId, context.CancellationToken);
        if (asset is null)
        {
            logger.LogWarning("Asset {AssetId} not found for processing completion.", msg.AssetId);
            return;
        }

        var result = asset.CompleteProcessing(
            msg.ThumbnailUrl,
            msg.TranscriptionText,
            msg.OutputUrls,
            msg.DurationSeconds);

        if (result.IsFailure)
        {
            logger.LogWarning("Could not complete asset {AssetId}: {Error}", msg.AssetId, result.Error.Message);
            return;
        }

        repository.Update(asset);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Asset {AssetId} marked as Completed.", msg.AssetId);
    }
}
