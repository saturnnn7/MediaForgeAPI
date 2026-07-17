namespace MediaForge.Identity.Application.Commands.RevokeCreator;

public sealed record RevokeCreatorCommand(Guid TargetUserId) : IRequest<Result>;
