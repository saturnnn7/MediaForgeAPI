using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetEdition;

public sealed record GetEditionQuery(Guid EditionId) : IRequest<Result<EditionDto>>;
