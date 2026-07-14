namespace MediaForge.Shared.Contracts;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OccurredOn { get; }
    Guid CorrelationId { get; }
}
