using MassTransit;
using MediaForge.Identity.Domain.Events;
using MediaForge.Shared.Contracts.Events.Identity;
using MediatR;

namespace MediaForge.Identity.Infrastructure.EventHandlers;

public sealed class UserRegisteredIntegrationEventPublisher(IPublishEndpoint publishEndpoint)
    : INotificationHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken) =>
        publishEndpoint.Publish(
            new UserRegisteredEvent(
                Guid.NewGuid(),
                DateTime.UtcNow,
                Guid.NewGuid(),
                notification.UserId,
                notification.Email,
                notification.DisplayName),
            cancellationToken);
}
