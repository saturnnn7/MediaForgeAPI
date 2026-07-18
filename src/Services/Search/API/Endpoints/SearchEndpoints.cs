using MediaForge.Search.Application.Queries.SearchMedia;
using MediaForge.Search.Application.Queries.SearchPersons;
using MediaForge.Search.Application.Queries.SearchWorks;
using MediatR;

namespace MediaForge.Search.API.Endpoints;

public static class SearchEndpoints
{
    public static IEndpointRouteBuilder MapSearchEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/search", async (string q, ISender sender, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new SearchMediaQuery(q, page, pageSize), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequireAuthorization();

        app.MapGet("/api/search/works", async (string q, ISender sender, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new SearchWorksQuery(q, page, pageSize), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequireAuthorization();

        app.MapGet("/api/search/persons", async (string q, ISender sender, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new SearchPersonsQuery(q, page, pageSize), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        })
        .RequireAuthorization();

        return app;
    }
}
