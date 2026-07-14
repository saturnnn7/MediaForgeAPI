namespace MediaForge.Shared.Contracts.Events.Media;

public sealed record MediaProcessingCompletedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid AssetId,
    Guid UserId,
    string? TranscriptionText,
    IReadOnlyList<string> OutputUrls,
    string ThumbnailUrl,
    double DurationSeconds
) : IIntegrationEvent;
