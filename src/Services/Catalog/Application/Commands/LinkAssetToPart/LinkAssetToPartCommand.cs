using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.LinkAssetToPart;

public sealed record LinkAssetToPartCommand(Guid PartId, Guid MediaAssetId, int SequenceOrder) : IRequest<Result<PartAssetDto>>;
