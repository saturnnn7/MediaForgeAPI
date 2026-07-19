using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkEditions;

public sealed record GetWorkEditionsQuery(Guid WorkId) : IRequest<Result<IReadOnlyList<EditionWithPartsDto>>>;
