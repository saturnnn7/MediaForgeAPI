namespace MediaForge.Catalog.Application.Commands.UnlinkAssetFromPart;

public sealed record UnlinkAssetFromPartCommand(Guid PartId, Guid MediaAssetId) : IRequest<Result>;
