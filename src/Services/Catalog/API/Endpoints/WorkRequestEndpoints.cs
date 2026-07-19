using MediaForge.Catalog.Application.Commands.ApproveWorkRequest;
using MediaForge.Catalog.Application.Commands.RejectWorkRequest;
using MediaForge.Catalog.Application.Commands.SubmitWorkRequest;
using MediaForge.Catalog.Application.Queries.GetMyWorkRequests;
using MediaForge.Catalog.Application.Queries.GetPendingWorkRequests;
using MediaForge.Catalog.Domain.Enums;

namespace MediaForge.Catalog.API.Endpoints;

public static class WorkRequestEndpoints
{
    public static IEndpointRouteBuilder MapWorkRequestEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/work-requests", async (SubmitWorkRequestRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new SubmitWorkRequestCommand(request.WorkType, request.Title, request.AuthorNames, request.Description, request.CoverUrl, request.Language);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/work-requests/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapGet("/api/work-requests/my", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyWorkRequestsQuery(), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapGet("/api/work-requests/pending", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPendingWorkRequestsQuery(), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/work-requests/{requestId:guid}/approve", async (Guid requestId, ApproveWorkRequestRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ApproveWorkRequestCommand(requestId, request.ChannelId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/work-requests/{requestId:guid}/reject", async (Guid requestId, RejectWorkRequestRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RejectWorkRequestCommand(requestId, request.AdminNote), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record SubmitWorkRequestRequest(WorkType WorkType, string Title, string? AuthorNames, string? Description, string? CoverUrl, string? Language);

    private sealed record ApproveWorkRequestRequest(Guid ChannelId);

    private sealed record RejectWorkRequestRequest(string? AdminNote);
}
