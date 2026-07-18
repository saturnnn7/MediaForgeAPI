namespace MediaForge.Search.Application.Queries.SearchWorks;

public sealed record SearchWorksQuery(string Query, int Page, int PageSize) : IRequest<Result<SearchResultDto<WorkDocument>>>;
