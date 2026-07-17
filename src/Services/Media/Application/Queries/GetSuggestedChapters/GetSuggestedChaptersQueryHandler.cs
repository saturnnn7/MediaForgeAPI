using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetSuggestedChapters;

public sealed class GetSuggestedChaptersQueryHandler(
    IMediaAssetRepository mediaAssetRepository,
    IMediaProcessingClient mediaProcessingClient) : IRequestHandler<GetSuggestedChaptersQuery, Result<IReadOnlyList<SuggestedChapterDto>>>
{
    public async Task<Result<IReadOnlyList<SuggestedChapterDto>>> Handle(GetSuggestedChaptersQuery request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<IReadOnlyList<SuggestedChapterDto>>(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.MediaType != MediaType.Audio)
            return Result.Failure<IReadOnlyList<SuggestedChapterDto>>(Error.Validation("MediaType", "Silence detection only works for audio files."));

        if (asset.Status != MediaAssetStatus.Completed)
            return Result.Failure<IReadOnlyList<SuggestedChapterDto>>(Error.Conflict("MediaAsset", "Asset processing is not completed."));

        var suggestions = await mediaProcessingClient.GetSuggestedChaptersAsync(asset.BucketName, asset.ObjectKey, cancellationToken);

        return Result.Success(suggestions);
    }
}
