namespace MediaForge.Shared.Contracts.Events.Media;

public sealed record MediaUploadedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid AssetId,
    Guid UserId,
    string BucketName,
    string ObjectKey,
    string ContentType,
    long FileSizeBytes
) : IIntegrationEvent;
