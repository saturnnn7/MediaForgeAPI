using Microsoft.Extensions.Logging;

namespace MediaForge.Search.Application.Consumers;

public sealed class WorkPublishedConsumer(
    ISearchService searchService,
    ILogger<WorkPublishedConsumer> logger) : IConsumer<WorkPublishedEvent>
{
    public async Task Consume(ConsumeContext<WorkPublishedEvent> context)
    {
        var doc = new WorkDocument
        {
            Id = context.Message.WorkId.ToString(),
            ChannelId = context.Message.ChannelId.ToString(),
            SeriesId = context.Message.SeriesId?.ToString(),
            Title = context.Message.Title,
            Description = context.Message.Description,
            WorkType = context.Message.WorkType,
            Language = context.Message.Language,
            CoverUrl = context.Message.CoverUrl,
            ContributorNames = context.Message.ContributorNames,
            GenreNames = context.Message.GenreNames,
            IndexedAt = DateTime.UtcNow
        };

        await searchService.IndexWorkAsync(doc, context.CancellationToken);

        logger.LogInformation("Indexed work document {WorkId}", context.Message.WorkId);
    }
}
