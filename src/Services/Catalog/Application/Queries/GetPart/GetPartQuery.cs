using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetPart;

public sealed record GetPartQuery(Guid PartId) : IRequest<Result<PartDetailDto>>;
