namespace MediaForge.Catalog.Application.Commands.UnlinkAssetFromPart;

public sealed class UnlinkAssetFromPartCommandHandler(
    IPartRepository partRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UnlinkAssetFromPartCommand, Result>
{
    public async Task<Result> Handle(UnlinkAssetFromPartCommand request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdWithDetailsAsync(request.PartId, cancellationToken);
        if (part is null)
            return Result.Failure(Error.NotFound("Part", request.PartId));

        var unlinkResult = part.UnlinkAsset(request.MediaAssetId);
        if (unlinkResult.IsFailure)
            return unlinkResult;

        await partRepository.RemoveAssetAsync(request.PartId, request.MediaAssetId, cancellationToken);
        partRepository.Update(part);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
