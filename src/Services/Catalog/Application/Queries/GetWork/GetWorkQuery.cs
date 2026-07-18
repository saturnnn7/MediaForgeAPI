using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWork;

public sealed record GetWorkQuery(Guid WorkId) : IRequest<Result<WorkDetailDto>>;
