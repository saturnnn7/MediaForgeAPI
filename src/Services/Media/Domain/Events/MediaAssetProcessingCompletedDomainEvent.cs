namespace MediaForge.Media.Domain.Events;

public sealed record MediaAssetProcessingCompletedDomainEvent(Guid AssetId, Guid UserId) : IDomainEvent, INotification;
