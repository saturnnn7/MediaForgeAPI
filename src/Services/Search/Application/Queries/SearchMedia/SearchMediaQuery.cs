namespace MediaForge.Search.Application.Queries.SearchMedia;

public sealed record SearchMediaQuery(string Query, int Page, int PageSize) : IRequest<Result<SearchResultDto<MediaDocument>>>;
