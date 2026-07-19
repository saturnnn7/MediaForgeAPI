using MediaForge.Catalog.Application.Commands.RefreshExternalRating;
using MediaForge.Catalog.Application.Commands.SetExternalRating;
using MediaForge.Catalog.Application.Queries.GetWorkExternalRatings;
using MediaForge.Catalog.Domain.Enums;

namespace MediaForge.Catalog.API.Endpoints;

public static class ExternalRatingEndpoints
{
    public static IEndpointRouteBuilder MapExternalRatingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/works/{workId:guid}/external-ratings", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkExternalRatingsQuery(workId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapPost("/api/works/{workId:guid}/external-ratings", async (Guid workId, SetExternalRatingRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new SetExternalRatingCommand(workId, request.Source, request.ExternalId, request.ExternalUrl);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/works/{workId:guid}/external-ratings/{source}/refresh", async (Guid workId, ExternalRatingSource source, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RefreshExternalRatingCommand(workId, source), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record SetExternalRatingRequest(ExternalRatingSource Source, string ExternalId, string? ExternalUrl);
}
