using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetUserMediaAssets;

public sealed class GetUserMediaAssetsQueryHandler(
    IMediaAssetRepository mediaAssetRepository,
    IUserContextService userContext) : IRequestHandler<GetUserMediaAssetsQuery, Result<IReadOnlyList<MediaAssetDto>>>
{
    public async Task<Result<IReadOnlyList<MediaAssetDto>>> Handle(GetUserMediaAssetsQuery request, CancellationToken cancellationToken)
    {
        var assets = await mediaAssetRepository.GetByUserIdAsync(userContext.UserId, request.Page, request.PageSize, cancellationToken);

        return Result.Success<IReadOnlyList<MediaAssetDto>>(assets.Select(a => a.ToDto()).ToList());
    }
}
