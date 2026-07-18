using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetPerson;

public sealed record GetPersonQuery(Guid PersonId) : IRequest<Result<PersonDto>>;
