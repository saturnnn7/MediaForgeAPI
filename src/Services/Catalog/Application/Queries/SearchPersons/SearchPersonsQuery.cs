using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.SearchPersons;

public sealed record SearchPersonsQuery(string Query, int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<PersonDto>>>;
