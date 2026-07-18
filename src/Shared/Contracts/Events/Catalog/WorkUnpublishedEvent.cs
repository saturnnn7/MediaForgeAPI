namespace MediaForge.Shared.Contracts.Events.Catalog;

public sealed record WorkUnpublishedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid WorkId
) : IIntegrationEvent;
