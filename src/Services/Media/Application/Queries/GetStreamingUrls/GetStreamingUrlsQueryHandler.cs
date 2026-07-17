using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetStreamingUrls;

public sealed class GetStreamingUrlsQueryHandler(
    IMediaAssetRepository mediaAssetRepository) : IRequestHandler<GetStreamingUrlsQuery, Result<StreamingUrlsDto>>
{
    private static readonly int[] KnownHeights = [1080, 720, 480];

    public async Task<Result<StreamingUrlsDto>> Handle(GetStreamingUrlsQuery request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<StreamingUrlsDto>(Error.NotFound("MediaAsset", request.AssetId));

        if (asset.Status != MediaAssetStatus.Completed)
            return Result.Failure<StreamingUrlsDto>(Error.Conflict("MediaAsset", "Asset processing is not completed."));

        var variants = asset.OutputUrls
            .Select(url => (Url: url, Height: KnownHeights.FirstOrDefault(h => url.Contains($"{h}p", StringComparison.OrdinalIgnoreCase))))
            .Where(x => x.Height != 0)
            .Select(x => new StreamingVariantDto(x.Height, x.Url, BitrateForHeight(x.Height)))
            .OrderByDescending(v => v.Height)
            .ToList();

        var dto = new StreamingUrlsDto(
            asset.Id,
            asset.ThumbnailUrl ?? string.Empty,
            variants,
            asset.SubtitleUrl,
            asset.DurationSeconds ?? 0,
            asset.WaveformUrl);

        return Result.Success(dto);
    }

    private static int BitrateForHeight(int height) => height switch
    {
        1080 => 5000,
        720 => 2800,
        480 => 1400,
        _ => 0
    };
}
