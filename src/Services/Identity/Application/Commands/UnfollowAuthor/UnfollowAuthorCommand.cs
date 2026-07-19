namespace MediaForge.Identity.Application.Commands.UnfollowAuthor;

public sealed record UnfollowAuthorCommand(Guid PersonId) : IRequest<Result>;
