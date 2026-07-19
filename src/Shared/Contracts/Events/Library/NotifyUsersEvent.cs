namespace MediaForge.Shared.Contracts.Events.Library;

public sealed record NotifyUsersEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    IReadOnlyList<Guid> UserIds,
    string NotificationType,
    string Payload
) : IIntegrationEvent;
