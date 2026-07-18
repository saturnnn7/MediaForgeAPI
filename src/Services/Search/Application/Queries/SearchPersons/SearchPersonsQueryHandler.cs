namespace MediaForge.Search.Application.Queries.SearchPersons;

public sealed class SearchPersonsQueryHandler(ISearchService searchService) : IRequestHandler<SearchPersonsQuery, Result<SearchResultDto<PersonDocument>>>
{
    public async Task<Result<SearchResultDto<PersonDocument>>> Handle(SearchPersonsQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Query))
            return Result.Failure<SearchResultDto<PersonDocument>>(Error.Validation("Query", "Search query is required."));

        var pageSize = request.PageSize > 50 ? 50 : request.PageSize;

        var result = await searchService.SearchPersonsAsync(request.Query, request.Page, pageSize, cancellationToken);

        return Result.Success(result);
    }
}
