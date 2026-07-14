using MassTransit;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Shared.Contracts.Events.Media;
using Microsoft.Extensions.Logging;

namespace MediaForge.Media.Infrastructure.Consumers;

public sealed class MediaProcessingFailedConsumer(
    IMediaAssetRepository repository,
    IMediaUnitOfWork unitOfWork,
    ILogger<MediaProcessingFailedConsumer> logger) : IConsumer<MediaProcessingFailedEvent>
{
    public async Task Consume(ConsumeContext<MediaProcessingFailedEvent> context)
    {
        var msg = context.Message;
        var asset = await repository.GetByIdAsync(msg.AssetId, context.CancellationToken);
        if (asset is null) return;

        asset.FailProcessing(msg.Reason);
        repository.Update(asset);
        await unitOfWork.SaveChangesAsync(context.CancellationToken);
        logger.LogWarning("Asset {AssetId} marked as Failed: {Reason}", msg.AssetId, msg.Reason);
    }
}
