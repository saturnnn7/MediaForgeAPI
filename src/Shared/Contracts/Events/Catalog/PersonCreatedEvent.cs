namespace MediaForge.Shared.Contracts.Events.Catalog;

public sealed record PersonCreatedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid PersonId,
    string Name,
    string? Bio,
    string? PhotoUrl
) : IIntegrationEvent;
