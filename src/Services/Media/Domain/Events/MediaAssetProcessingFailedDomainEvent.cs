namespace MediaForge.Media.Domain.Events;

public sealed record MediaAssetProcessingFailedDomainEvent(Guid AssetId, Guid UserId, string Reason) : IDomainEvent, INotification;
