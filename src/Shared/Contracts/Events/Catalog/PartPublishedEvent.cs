namespace MediaForge.Shared.Contracts.Events.Catalog;

public sealed record PartPublishedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid PartId,
    Guid EditionId,
    Guid WorkId,
    string PartTitle,
    int OrderMajor,
    int OrderMinor
) : IIntegrationEvent;
