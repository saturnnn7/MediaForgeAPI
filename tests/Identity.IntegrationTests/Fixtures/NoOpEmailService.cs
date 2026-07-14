using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.IntegrationTests.Fixtures;

public sealed class NoOpEmailService : IEmailService
{
    public Task SendVerificationEmailAsync(string email, string displayName, string verificationLink, CancellationToken ct) =>
        Task.CompletedTask;

    public Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken ct) =>
        Task.CompletedTask;
}
