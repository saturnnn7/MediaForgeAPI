using Microsoft.Extensions.Logging;

namespace MediaForge.Search.Application.Consumers;

public sealed class MediaProcessingCompletedConsumer(
    ISearchService searchService,
    ILogger<MediaProcessingCompletedConsumer> logger) : IConsumer<MediaProcessingCompletedEvent>
{
    public async Task Consume(ConsumeContext<MediaProcessingCompletedEvent> context)
    {
        var outputUrls = context.Message.OutputUrls;
        var firstOutputUrl = outputUrls.Count > 0 ? outputUrls[0] : "untitled";

        var doc = new MediaDocument
        {
            Id = context.Message.AssetId.ToString(),
            UserId = context.Message.UserId.ToString(),
            Title = Path.GetFileNameWithoutExtension(firstOutputUrl),
            TranscriptionText = context.Message.TranscriptionText,
            MediaType = context.Message.DurationSeconds > 0 ? "media" : "unknown",
            ThumbnailUrl = context.Message.ThumbnailUrl,
            OutputUrls = context.Message.OutputUrls,
            DurationSeconds = context.Message.DurationSeconds,
            CreatedAt = context.Message.OccurredOn,
            IndexedAt = DateTime.UtcNow
        };

        await searchService.IndexAsync(doc, context.CancellationToken);

        logger.LogInformation("Indexed media document {AssetId}", context.Message.AssetId);
    }
}
