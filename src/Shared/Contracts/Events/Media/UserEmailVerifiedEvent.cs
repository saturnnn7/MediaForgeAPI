namespace MediaForge.Shared.Contracts.Events.Identity;

public sealed record UserEmailVerifiedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid UserId,
    string Email
) : IIntegrationEvent;
