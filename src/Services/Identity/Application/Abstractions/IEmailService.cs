namespace MediaForge.Identity.Application.Abstractions;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string displayName, string verificationLink, CancellationToken ct);
    Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken ct);
}
