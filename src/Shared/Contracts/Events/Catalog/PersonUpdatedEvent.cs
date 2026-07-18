namespace MediaForge.Shared.Contracts.Events.Catalog;

public sealed record PersonUpdatedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid PersonId,
    string Name,
    string? Bio,
    string? PhotoUrl
) : IIntegrationEvent;
