namespace MediaForge.Identity.Domain.Events;

public sealed record UserEmailVerifiedDomainEvent(Guid UserId, string Email) : IDomainEvent, INotification;
