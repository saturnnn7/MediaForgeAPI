using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.LinkAssetToPart;

public sealed class LinkAssetToPartCommandHandler(
    IPartRepository partRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<LinkAssetToPartCommand, Result<PartAssetDto>>
{
    public async Task<Result<PartAssetDto>> Handle(LinkAssetToPartCommand request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdWithDetailsAsync(request.PartId, cancellationToken);
        if (part is null)
            return Result.Failure<PartAssetDto>(Error.NotFound("Part", request.PartId));

        var linkResult = part.LinkAsset(request.MediaAssetId, request.SequenceOrder);
        if (linkResult.IsFailure)
            return Result.Failure<PartAssetDto>(linkResult.Error);

        await partRepository.AddAssetAsync(linkResult.Value, cancellationToken);
        partRepository.Update(part);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(linkResult.Value.ToDto());
    }
}
