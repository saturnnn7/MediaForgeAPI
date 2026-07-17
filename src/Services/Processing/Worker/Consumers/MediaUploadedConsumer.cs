using MediaForge.Processing.Worker.Services;

namespace MediaForge.Processing.Worker.Consumers;

public sealed class MediaUploadedConsumer(
    IMediaProcessingService processingService,
    IStorageService storageService,
    ILogger<MediaUploadedConsumer> logger) : IConsumer<MediaUploadedEvent>
{
    public async Task Consume(ConsumeContext<MediaUploadedEvent> context)
    {
        logger.LogInformation("Starting processing for asset {AssetId}", context.Message.AssetId);

        var localDir = Path.Combine(Path.GetTempPath(), "mediaforge", "downloads", context.Message.AssetId.ToString());
        Directory.CreateDirectory(localDir);
        var ext = Path.GetExtension(context.Message.ObjectKey);
        var localFilePath = Path.Combine(localDir, $"source{ext}");

        try
        {
            await storageService.DownloadToFileAsync(context.Message.BucketName, context.Message.ObjectKey, localFilePath, context.CancellationToken);

            var result = await processingService.ProcessAsync(
                localFilePath,
                context.Message.ContentType,
                Path.GetFileName(context.Message.ObjectKey),
                "mediaforge-processed",
                context.Message.AssetId.ToString(),
                context.CancellationToken);

            await context.Publish(new MediaProcessingCompletedEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                context.Message.CorrelationId,
                context.Message.AssetId,
                context.Message.UserId,
                result.TranscriptionText,
                result.OutputUrls,
                result.ThumbnailUrl,
                result.DurationSeconds,
                result.WaveformUrl,
                result.SubtitleUrl));

            logger.LogInformation("Completed processing for asset {AssetId}", context.Message.AssetId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Processing failed for asset {AssetId}", context.Message.AssetId);

            await context.Publish(new MediaProcessingFailedEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                context.Message.CorrelationId,
                context.Message.AssetId,
                context.Message.UserId,
                ex.Message));

            throw;
        }
        finally
        {
            if (Directory.Exists(localDir))
                Directory.Delete(localDir, recursive: true);
        }
    }
}
