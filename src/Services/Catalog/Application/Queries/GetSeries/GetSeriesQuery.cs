using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetSeries;

public sealed record GetSeriesQuery(Guid SeriesId) : IRequest<Result<SeriesDto>>;
