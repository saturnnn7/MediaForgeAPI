using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetChannelSeries;

public sealed record GetChannelSeriesQuery(Guid ChannelId, int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<SeriesDto>>>;
