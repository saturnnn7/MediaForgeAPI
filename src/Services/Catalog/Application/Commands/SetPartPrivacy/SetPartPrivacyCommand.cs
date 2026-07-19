namespace MediaForge.Catalog.Application.Commands.SetPartPrivacy;

public sealed record SetPartPrivacyCommand(Guid PartId, bool IsPrivate) : IRequest<Result>;
