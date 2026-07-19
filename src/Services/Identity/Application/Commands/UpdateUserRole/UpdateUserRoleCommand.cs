namespace MediaForge.Identity.Application.Commands.UpdateUserRole;

public sealed record UpdateUserRoleCommand(Guid UserId, UserRole Role) : IRequest<Result>;
