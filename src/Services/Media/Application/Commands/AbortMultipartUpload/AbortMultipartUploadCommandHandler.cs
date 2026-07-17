using MediaForge.Media.Application.Abstractions;

namespace MediaForge.Media.Application.Commands.AbortMultipartUpload;

public sealed class AbortMultipartUploadCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IStorageService storageService,
    IMediaUnitOfWork unitOfWork) : IRequestHandler<AbortMultipartUploadCommand, Result>
{
    public async Task<Result> Handle(AbortMultipartUploadCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure(Error.Unauthorized("You do not have access to this media asset."));

        await storageService.AbortMultipartUploadAsync(asset.BucketName, asset.ObjectKey, request.UploadId, cancellationToken);

        var result = asset.Cancel();
        if (result.IsFailure)
            return result;

        mediaAssetRepository.Update(asset);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
