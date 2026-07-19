using MediaForge.Library.Application.Commands.AddToLibrary;
using MediaForge.Library.Application.Commands.RateWork;
using MediaForge.Library.Application.Commands.RemoveFromLibrary;
using MediaForge.Library.Application.Commands.ToggleFavorite;
using MediaForge.Library.Application.Commands.UpdateLibraryStatus;
using MediaForge.Library.Application.Queries.GetMyLibrary;
using MediaForge.Library.Domain.Enums;

namespace MediaForge.Library.API.Endpoints;

public static class LibraryEntryEndpoints
{
    public static IEndpointRouteBuilder MapLibraryEntryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/library", async (AddToLibraryRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddToLibraryCommand(request.WorkId, request.Status, request.Privacy ?? ListPrivacy.Everyone);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/library/{result.Value.WorkId}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPut("/api/library/{workId:guid}/status", async (Guid workId, UpdateLibraryStatusRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UpdateLibraryStatusCommand(workId, request.NewStatus), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/library/{workId:guid}/favorite", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ToggleFavoriteCommand(workId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPut("/api/library/{workId:guid}/rating", async (Guid workId, RateWorkRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RateWorkCommand(workId, request.Rating), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapDelete("/api/library/{workId:guid}", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveFromLibraryCommand(workId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapGet("/api/library", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyLibraryQuery(), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record AddToLibraryRequest(Guid WorkId, LibraryStatus Status, ListPrivacy? Privacy);

    private sealed record UpdateLibraryStatusRequest(LibraryStatus NewStatus);

    private sealed record RateWorkRequest(int? Rating);
}
