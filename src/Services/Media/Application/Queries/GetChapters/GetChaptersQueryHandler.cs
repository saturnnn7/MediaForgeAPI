using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetChapters;

public sealed class GetChaptersQueryHandler(
    IMediaAssetRepository mediaAssetRepository) : IRequestHandler<GetChaptersQuery, Result<IReadOnlyList<ChapterDto>>>
{
    public async Task<Result<IReadOnlyList<ChapterDto>>> Handle(GetChaptersQuery request, CancellationToken cancellationToken)
    {
        var asset = await mediaAssetRepository.GetByIdWithChaptersAsync(request.AssetId, cancellationToken);
        if (asset is null)
            return Result.Failure<IReadOnlyList<ChapterDto>>(Error.NotFound("MediaAsset", request.AssetId));

        var chapters = asset.Chapters
            .OrderBy(c => c.Order)
            .Select(c => c.ToDto())
            .ToList();

        return Result.Success<IReadOnlyList<ChapterDto>>(chapters);
    }
}
