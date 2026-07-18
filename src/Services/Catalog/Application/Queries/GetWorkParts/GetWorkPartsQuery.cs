using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkParts;

public sealed record GetWorkPartsQuery(Guid WorkId) : IRequest<Result<IReadOnlyList<PartSummaryDto>>>;
