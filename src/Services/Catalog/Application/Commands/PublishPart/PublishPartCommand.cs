namespace MediaForge.Catalog.Application.Commands.PublishPart;

public sealed record PublishPartCommand(Guid PartId) : IRequest<Result>;
