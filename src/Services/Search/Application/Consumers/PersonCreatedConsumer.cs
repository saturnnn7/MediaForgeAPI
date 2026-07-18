using Microsoft.Extensions.Logging;

namespace MediaForge.Search.Application.Consumers;

public sealed class PersonCreatedConsumer(
    ISearchService searchService,
    ILogger<PersonCreatedConsumer> logger) : IConsumer<PersonCreatedEvent>
{
    public async Task Consume(ConsumeContext<PersonCreatedEvent> context)
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
