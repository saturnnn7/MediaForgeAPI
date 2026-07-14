namespace MediaForge.Shared.Contracts.Events.Media;

public sealed record MediaProcessingStartedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid AssetId,
    Guid UserId
) : IIntegrationEvent;
