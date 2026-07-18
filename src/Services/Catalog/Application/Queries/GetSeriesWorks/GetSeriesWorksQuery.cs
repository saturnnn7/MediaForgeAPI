using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetSeriesWorks;

public sealed record GetSeriesWorksQuery(Guid SeriesId) : IRequest<Result<IReadOnlyList<WorkSummaryDto>>>;
