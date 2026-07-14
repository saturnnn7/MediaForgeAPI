namespace MediaForge.Media.Domain.Events;

public sealed record MediaAssetUploadedDomainEvent(
    Guid AssetId,
    Guid UserId,
    string BucketName,
    string ObjectKey,
    string ContentType,
    long FileSizeBytes) : IDomainEvent, INotification;
