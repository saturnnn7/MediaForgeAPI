using MediaForge.Catalog.Application.Commands.CreateEdition;
using MediaForge.Catalog.Application.Commands.SetDefaultEdition;
using MediaForge.Catalog.Application.Commands.UpdateEdition;
using MediaForge.Catalog.Application.Queries.GetEdition;
using MediaForge.Catalog.Application.Queries.GetWorkEditions;

namespace MediaForge.Catalog.API.Endpoints;

public static class EditionEndpoints
{
    public static IEndpointRouteBuilder MapEditionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/works/{workId:guid}/editions", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkEditionsQuery(workId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapGet("/api/editions/{editionId:guid}", async (Guid editionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetEditionQuery(editionId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapPost("/api/editions", async (CreateEditionRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateEditionCommand(request.WorkId, request.NarratorTeamName, request.Description, request.CoverUrl, request.Language ?? "en");
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/editions/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPut("/api/editions/{editionId:guid}", async (Guid editionId, UpdateEditionRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateEditionCommand(editionId, request.NarratorTeamName, request.Description, request.CoverUrl, request.Language);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/editions/{editionId:guid}/set-default", async (Guid editionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SetDefaultEditionCommand(editionId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record CreateEditionRequest(Guid WorkId, string NarratorTeamName, string? Description, string? CoverUrl, string? Language);

    private sealed record UpdateEditionRequest(string NarratorTeamName, string? Description, string? CoverUrl, string? Language);
}
