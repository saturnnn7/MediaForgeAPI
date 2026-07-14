namespace MediaForge.Identity.Application.Commands.VerifyEmail;

public sealed record VerifyEmailCommand(Guid UserId, string Token) : IRequest<Result>;
