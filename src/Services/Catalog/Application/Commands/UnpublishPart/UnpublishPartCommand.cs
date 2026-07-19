namespace MediaForge.Catalog.Application.Commands.UnpublishPart;

public sealed record UnpublishPartCommand(Guid PartId) : IRequest<Result>;
