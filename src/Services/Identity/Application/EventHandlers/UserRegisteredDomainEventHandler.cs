using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.EventHandlers;

public sealed class UserRegisteredDomainEventHandler(IEmailService emailService, ITokenService tokenService)
    : INotificationHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        var token = await tokenService.GenerateEmailVerificationTokenAsync(notification.UserId, cancellationToken);
        var verificationLink = $"https://mediaforge.local/verify-email?userId={notification.UserId}&token={token}";

        await emailService.SendVerificationEmailAsync(
            notification.Email,
            notification.DisplayName,
            verificationLink,
            cancellationToken);
    }
}
