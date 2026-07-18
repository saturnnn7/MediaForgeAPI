using MediaForge.Catalog.Application.Commands.AddContributor;
using MediaForge.Catalog.Application.Commands.AddGenreToWork;
using MediaForge.Catalog.Application.Commands.CreateWork;
using MediaForge.Catalog.Application.Commands.PublishWork;
using MediaForge.Catalog.Application.Commands.RemoveContributor;
using MediaForge.Catalog.Application.Commands.RemoveGenreFromWork;
using MediaForge.Catalog.Application.Commands.UnpublishWork;
using MediaForge.Catalog.Application.Commands.UpdateWork;
using MediaForge.Catalog.Application.Queries.GetChannelWorks;
using MediaForge.Catalog.Application.Queries.GetSeriesWorks;
using MediaForge.Catalog.Application.Queries.GetWork;
using MediaForge.Catalog.Domain.Enums;

namespace MediaForge.Catalog.API.Endpoints;

public static class WorkEndpoints
{
    public static IEndpointRouteBuilder MapWorkEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/works/{workId:guid}", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkQuery(workId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapGet("/api/channels/{channelId:guid}/works", async (Guid channelId, ISender sender, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var result = await sender.Send(new GetChannelWorksQuery(channelId, page, Math.Min(pageSize, 50)), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapGet("/api/series/{seriesId:guid}/works", async (Guid seriesId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetSeriesWorksQuery(seriesId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapPost("/api/works", async (CreateWorkCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/works/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPut("/api/works/{workId:guid}", async (Guid workId, UpdateWorkRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateWorkCommand(workId, request.Title, request.Description, request.CoverUrl, request.Language);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/works/{workId:guid}/publish", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new PublishWorkCommand(workId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/works/{workId:guid}/unpublish", async (Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UnpublishWorkCommand(workId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/works/{workId:guid}/contributors", async (Guid workId, AddContributorRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddContributorCommand(workId, request.PersonId, request.Role, request.DisplayOrder);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapDelete("/api/works/{workId:guid}/contributors/{personId:guid}/{role}", async (Guid workId, Guid personId, ContributorRole role, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveContributorCommand(workId, personId, role), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPost("/api/works/{workId:guid}/genres/{genreId:guid}", async (Guid workId, Guid genreId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AddGenreToWorkCommand(workId, genreId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapDelete("/api/works/{workId:guid}/genres/{genreId:guid}", async (Guid workId, Guid genreId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveGenreFromWorkCommand(workId, genreId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record UpdateWorkRequest(string Title, string? Description, string? CoverUrl, string? Language);

    private sealed record AddContributorRequest(Guid PersonId, ContributorRole Role, int DisplayOrder = 0);
}
