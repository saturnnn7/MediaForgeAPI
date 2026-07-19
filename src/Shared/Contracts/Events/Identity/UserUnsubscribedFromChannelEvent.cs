namespace MediaForge.Shared.Contracts.Events.Identity;

public sealed record UserUnsubscribedFromChannelEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid ChannelId,
    Guid SubscriberId
) : IIntegrationEvent;
