using Microsoft.Extensions.Logging;

namespace MediaForge.Search.Application.Consumers;

public sealed class WorkUnpublishedConsumer(
    ISearchService searchService,
    ILogger<WorkUnpublishedConsumer> logger) : IConsumer<WorkUnpublishedEvent>
{
    public async Task Consume(ConsumeContext<WorkUnpublishedEvent> context)
    {
        await searchService.DeleteWorkAsync(context.Message.WorkId.ToString(), context.CancellationToken);

        logger.LogInformation("Deleted work document {WorkId}", context.Message.WorkId);
    }
}
