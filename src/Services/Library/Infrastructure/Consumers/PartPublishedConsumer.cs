using System.Text.Json;
using MassTransit;
using MediaForge.Shared.Contracts.Events.Catalog;
using MediaForge.Shared.Contracts.Events.Library;
using Microsoft.Extensions.Logging;

namespace MediaForge.Library.Infrastructure.Consumers;

public sealed class PartPublishedConsumer(
    ILibraryEntryRepository libraryEntryRepository,
    ILogger<PartPublishedConsumer> logger) : IConsumer<PartPublishedEvent>
{
    public async Task Consume(ConsumeContext<PartPublishedEvent> context)
    {
        var message = context.Message;

        var entries = await libraryEntryRepository.GetByWorkIdAsync(message.WorkId, context.CancellationToken);

        var userIds = entries
            .Where(e => e.Status != LibraryStatus.Abandoned)
            .Select(e => e.UserId)
            .ToList();

        if (userIds.Count == 0)
        {
            logger.LogInformation("No library users to notify for work {WorkId}", message.WorkId);
            return;
        }

        await context.Publish(new NotifyUsersEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            message.CorrelationId,
            userIds,
            "NewPartAdded",
            JsonSerializer.Serialize(new
            {
                workId = message.WorkId,
                partId = message.PartId,
                partTitle = message.PartTitle,
                orderMajor = message.OrderMajor,
                orderMinor = message.OrderMinor
            })));
    }
}
