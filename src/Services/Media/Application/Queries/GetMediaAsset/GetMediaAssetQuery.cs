using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetMediaAsset;

public sealed record GetMediaAssetQuery(Guid AssetId) : IRequest<Result<MediaAssetDto>>;
