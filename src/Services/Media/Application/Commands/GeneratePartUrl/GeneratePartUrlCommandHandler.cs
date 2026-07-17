using MediaForge.Media.Application.Abstractions;

namespace MediaForge.Media.Application.Commands.GeneratePartUrl;

public sealed class GeneratePartUrlCommandHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext,
    IStorageService storageService) : IRequestHandler<GeneratePartUrlCommand, Result<string>>
{
    public async Task<Result<string>> Handle(GeneratePartUrlCommand request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<string>(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure<string>(Error.Unauthorized("You do not have access to this media asset."));

        if (request.PartNumber < 1 || request.PartNumber > 10000)
            return Result.Failure<string>(Error.Validation("PartNumber", "Part number must be between 1 and 10000."));

        var url = await storageService.GeneratePartUploadUrlAsync(
            asset.BucketName, asset.ObjectKey, request.UploadId, request.PartNumber, cancellationToken);

        return Result.Success(url);
    }
}
