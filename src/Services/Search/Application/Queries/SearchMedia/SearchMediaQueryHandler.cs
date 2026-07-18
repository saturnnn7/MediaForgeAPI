namespace MediaForge.Search.Application.Queries.SearchMedia;

public sealed class SearchMediaQueryHandler(ISearchService searchService) : IRequestHandler<SearchMediaQuery, Result<SearchResultDto<MediaDocument>>>
{
    public async Task<Result<SearchResultDto<MediaDocument>>> Handle(SearchMediaQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Query))
            return Result.Failure<SearchResultDto<MediaDocument>>(Error.Validation("Query", "Search query is required."));

        var pageSize = request.PageSize > 50 ? 50 : request.PageSize;

        var result = await searchService.SearchAsync(request.Query, request.Page, pageSize, cancellationToken);

        return Result.Success(result);
    }
}
