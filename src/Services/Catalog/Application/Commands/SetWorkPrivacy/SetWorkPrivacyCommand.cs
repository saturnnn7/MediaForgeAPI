namespace MediaForge.Catalog.Application.Commands.SetWorkPrivacy;

public sealed record SetWorkPrivacyCommand(Guid WorkId, bool IsPrivate) : IRequest<Result>;
