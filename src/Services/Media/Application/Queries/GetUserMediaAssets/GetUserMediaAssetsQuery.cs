using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetUserMediaAssets;

public sealed record GetUserMediaAssetsQuery(int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<MediaAssetDto>>>;
