using MediaForge.Library.Application.Commands.UpdateProgress;
using MediaForge.Library.Application.Queries.GetProgress;

namespace MediaForge.Library.API.Endpoints;

public static class ListeningProgressEndpoints
{
    public static IEndpointRouteBuilder MapListeningProgressEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/progress/{editionId:guid}/{partId:guid}", async (Guid editionId, Guid partId, UpdateProgressRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateProgressCommand(editionId, partId, request.PositionSeconds), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapGet("/api/progress/{editionId:guid}", async (Guid editionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetProgressQuery(editionId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record UpdateProgressRequest(double PositionSeconds);
}
