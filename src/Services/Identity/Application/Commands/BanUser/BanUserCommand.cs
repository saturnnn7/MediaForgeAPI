namespace MediaForge.Identity.Application.Commands.BanUser;

public sealed record BanUserCommand(Guid UserId, bool IsBanned, string? Reason) : IRequest<Result>;
