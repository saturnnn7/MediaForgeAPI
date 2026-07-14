namespace MediaForge.Shared.Contracts.Events.Media;

public sealed record MediaProcessingFailedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid AssetId,
    Guid UserId,
    string Reason
) : IIntegrationEvent;
