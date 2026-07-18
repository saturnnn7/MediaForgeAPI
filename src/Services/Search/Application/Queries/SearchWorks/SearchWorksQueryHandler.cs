namespace MediaForge.Search.Application.Queries.SearchWorks;

public sealed class SearchWorksQueryHandler(ISearchService searchService) : IRequestHandler<SearchWorksQuery, Result<SearchResultDto<WorkDocument>>>
{
    public async Task<Result<SearchResultDto<WorkDocument>>> Handle(SearchWorksQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Query))
            return Result.Failure<SearchResultDto<WorkDocument>>(Error.Validation("Query", "Search query is required."));

        var pageSize = request.PageSize > 50 ? 50 : request.PageSize;

        var result = await searchService.SearchWorksAsync(request.Query, request.Page, pageSize, cancellationToken);

        return Result.Success(result);
    }
}
