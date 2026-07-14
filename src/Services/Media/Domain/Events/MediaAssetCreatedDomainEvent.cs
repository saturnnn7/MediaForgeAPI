namespace MediaForge.Media.Domain.Events;

public sealed record MediaAssetCreatedDomainEvent(Guid AssetId, Guid UserId) : IDomainEvent, INotification;
