using MediaForge.Media.Application.DTOs;

namespace MediaForge.Media.Application.Queries.GetStreamingUrls;

public sealed record GetStreamingUrlsQuery(Guid AssetId) : IRequest<Result<StreamingUrlsDto>>;
