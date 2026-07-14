namespace MediaForge.Shared.Contracts.Events.Identity;

public sealed record UserRegisteredEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid UserId,
    string Email,
    string DisplayName
) : IIntegrationEvent;
