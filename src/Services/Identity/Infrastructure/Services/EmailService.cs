using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace MediaForge.Identity.Infrastructure.Services;

public sealed class EmailService(IConfiguration configuration) : IEmailService
{
    public Task SendVerificationEmailAsync(string email, string displayName, string verificationLink, CancellationToken ct)
    {
        var body = $"""
            <p>Hi {displayName},</p>
            <p>Please verify your email by clicking <a href="{verificationLink}">here</a>.</p>
            """;

        return SendAsync(email, "Verify your email", body, ct);
    }

    public Task SendPasswordResetEmailAsync(string email, string resetLink, CancellationToken ct)
    {
        var body = $"""
            <p>You requested a password reset. Click <a href="{resetLink}">here</a> to reset your password.</p>
            <p>If you did not request this, you can ignore this email.</p>
            """;

        return SendAsync(email, "Reset your password", body, ct);
    }

    private async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct)
    {
        var host = configuration["Email:Host"]!;
        var port = int.Parse(configuration["Email:Port"]!, CultureInfo.InvariantCulture);
        var username = configuration["Email:Username"]!;
        var password = configuration["Email:Password"]!;
        var from = configuration["Email:From"]!;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = true
        };

        using var message = new MailMessage(from, toEmail, subject, htmlBody)
        {
            IsBodyHtml = true
        };

        try {
            await client.SendMailAsync(message, ct);
        }
        catch (Exception ex)
        {
            // Log and continue — email failure should not fail registration in dev
            Console.WriteLine($"[EmailService] Failed to send email to {toEmail}: {ex.Message}");
        }
    }
}
