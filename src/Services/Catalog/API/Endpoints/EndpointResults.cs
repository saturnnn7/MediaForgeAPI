namespace MediaForge.Catalog.API.Endpoints;

internal static class EndpointResults
{
    public static IResult ToHttpResult<T>(Result<T> result) => result.IsSuccess
        ? Results.Ok(result.Value)
        : result.Error.Code.Contains("NotFound") ? Results.NotFound(result.Error)
        : result.Error.Code.Contains("Unauthorized") ? Results.Unauthorized()
        : result.Error.Code.Contains("Conflict") ? Results.Conflict(result.Error)
        : Results.BadRequest(result.Error);

    public static IResult ToHttpResult(Result result) => result.IsSuccess
        ? Results.Ok()
        : result.Error.Code.Contains("NotFound") ? Results.NotFound(result.Error)
        : result.Error.Code.Contains("Unauthorized") ? Results.Unauthorized()
        : result.Error.Code.Contains("Conflict") ? Results.Conflict(result.Error)
        : Results.BadRequest(result.Error);
}
