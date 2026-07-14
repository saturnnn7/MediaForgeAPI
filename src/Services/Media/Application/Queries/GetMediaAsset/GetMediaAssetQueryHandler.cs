using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetMediaAsset;

public sealed class GetMediaAssetQueryHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext) : IRequestHandler<GetMediaAssetQuery, Result<MediaAssetDto>>
{
    public async Task<Result<MediaAssetDto>> Handle(GetMediaAssetQuery request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<MediaAssetDto>(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.UserId != userContext.UserId)
            return Result.Failure<MediaAssetDto>(Error.Unauthorized("You do not have access to this media asset."));

        return Result.Success(asset.ToDto());
    }
}
