using MediaForge.Catalog.Application.Commands.AddChapterToPart;
using MediaForge.Catalog.Application.Commands.CreatePart;
using MediaForge.Catalog.Application.Commands.LinkAssetToPart;
using MediaForge.Catalog.Application.Commands.RemoveChapterFromPart;
using MediaForge.Catalog.Application.Commands.SetPartPrivacy;
using MediaForge.Catalog.Application.Commands.UnlinkAssetFromPart;
using MediaForge.Catalog.Application.Commands.UpdatePart;
using MediaForge.Catalog.Application.Queries.GetPart;
using MediaForge.Catalog.Application.Queries.GetWorkParts;
using MediaForge.Catalog.Domain.Enums;

namespace MediaForge.Catalog.API.Endpoints;

public static class PartEndpoints
{
    public static IEndpointRouteBuilder MapPartEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/parts/{partId:guid}", async (Guid partId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPartQuery(partId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapGet("/api/works/{workId:guid}/parts", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkPartsQuery(workId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapPost("/api/parts", async (CreatePartRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreatePartCommand(request.WorkId, request.Title, request.Description, request.OrderMajor, request.OrderMinor, request.PartType, request.CoverUrl);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/parts/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPut("/api/parts/{partId:guid}", async (Guid partId, UpdatePartRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdatePartCommand(partId, request.Title, request.Description, request.CoverUrl);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPatch("/api/parts/{partId:guid}/privacy", async (Guid partId, SetPrivacyRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SetPartPrivacyCommand(partId, request.IsPrivate), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/parts/{partId:guid}/chapters", async (Guid partId, AddChapterRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddChapterToPartCommand(partId, request.Title, request.StartTimeSeconds, request.EndTimeSeconds, request.Order);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/parts/{partId}/chapters", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapDelete("/api/parts/{partId:guid}/chapters/{chapterId:guid}", async (Guid partId, Guid chapterId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveChapterFromPartCommand(partId, chapterId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/parts/{partId:guid}/assets", async (Guid partId, LinkAssetRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new LinkAssetToPartCommand(partId, request.MediaAssetId, request.SequenceOrder);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/parts/{partId}/assets", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapDelete("/api/parts/{partId:guid}/assets/{mediaAssetId:guid}", async (Guid partId, Guid mediaAssetId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UnlinkAssetFromPartCommand(partId, mediaAssetId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record CreatePartRequest(Guid WorkId, string Title, string? Description, int OrderMajor, int OrderMinor, PartType PartType, string? CoverUrl);

    private sealed record UpdatePartRequest(string Title, string? Description, string? CoverUrl);

    private sealed record SetPrivacyRequest(bool IsPrivate);

    private sealed record AddChapterRequest(string Title, double StartTimeSeconds, double EndTimeSeconds, int Order);

    private sealed record LinkAssetRequest(Guid MediaAssetId, int SequenceOrder);
}
