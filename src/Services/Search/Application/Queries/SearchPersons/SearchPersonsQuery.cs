namespace MediaForge.Search.Application.Queries.SearchPersons;

public sealed record SearchPersonsQuery(string Query, int Page, int PageSize) : IRequest<Result<SearchResultDto<PersonDocument>>>;
