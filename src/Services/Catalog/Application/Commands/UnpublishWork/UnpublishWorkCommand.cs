namespace MediaForge.Catalog.Application.Commands.UnpublishWork;

public sealed record UnpublishWorkCommand(Guid WorkId) : IRequest<Result>;
