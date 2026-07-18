using MediaForge.Catalog.Application.Commands.CreateSeries;
using MediaForge.Catalog.Application.Commands.UpdateSeries;
using MediaForge.Catalog.Application.Queries.GetChannelSeries;
using MediaForge.Catalog.Application.Queries.GetSeries;

namespace MediaForge.Catalog.API.Endpoints;

public static class SeriesEndpoints
{
    public static IEndpointRouteBuilder MapSeriesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/series/{seriesId:guid}", async (Guid seriesId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSeriesQuery(seriesId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapGet("/api/channels/{channelId:guid}/series", async (Guid channelId, ISender sender, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new GetChannelSeriesQuery(channelId, page, Math.Min(pageSize, 50)), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapPost("/api/series", async (CreateSeriesCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/series/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPut("/api/series/{seriesId:guid}", async (Guid seriesId, UpdateSeriesRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateSeriesCommand(seriesId, request.Title, request.Description, request.CoverUrl);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record UpdateSeriesRequest(string Title, string? Description, string? CoverUrl);
}
