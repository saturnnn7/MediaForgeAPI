namespace MediaForge.Identity.Domain.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId, string Email, string DisplayName) : IDomainEvent, INotification;
