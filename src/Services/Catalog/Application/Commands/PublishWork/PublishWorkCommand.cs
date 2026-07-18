namespace MediaForge.Catalog.Application.Commands.PublishWork;

public sealed record PublishWorkCommand(Guid WorkId) : IRequest<Result>;
