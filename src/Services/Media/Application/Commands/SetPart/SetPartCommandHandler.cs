using MediaForge.Media.Application.Abstractions;

namespace MediaForge.Media.Application.Commands.SetPart;

public sealed class SetPartCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IMediaUnitOfWork unitOfWork) : IRequestHandler<SetPartCommand, Result>
{
    public async Task<Result> Handle(SetPartCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure(Error.Unauthorized("You do not have access to this media asset."));

        asset.SetPartId(request.PartId);
        mediaAssetRepository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
