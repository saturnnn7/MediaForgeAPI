using Microsoft.Extensions.Logging;

namespace MediaForge.Search.Application.Consumers;

public sealed class PersonUpdatedConsumer(
    ISearchService searchService,
    ILogger<PersonUpdatedConsumer> logger) : IConsumer<PersonUpdatedEvent>
{
    public async Task Consume(ConsumeContext<PersonUpdatedEvent> context)
    {
        var doc = new PersonDocument
        {
            Id = context.Message.PersonId.ToString(),
            Name = context.Message.Name,
            Bio = context.Message.Bio,
            PhotoUrl = context.Message.PhotoUrl,
            IndexedAt = DateTime.UtcNow
        };

        await searchService.IndexPersonAsync(doc, context.CancellationToken);

        logger.LogInformation("Indexed person document {PersonId}", context.Message.PersonId);
    }
}
