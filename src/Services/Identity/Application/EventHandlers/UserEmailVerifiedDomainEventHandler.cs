using Microsoft.Extensions.Logging;

namespace MediaForge.Identity.Application.EventHandlers;

public sealed class UserEmailVerifiedDomainEventHandler(ILogger<UserEmailVerifiedDomainEventHandler> logger)
    : INotificationHandler<UserEmailVerifiedDomainEvent>
{
    public Task Handle(UserEmailVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Email verified for user {UserId}", notification.UserId);

        return Task.CompletedTask;
    }
}
