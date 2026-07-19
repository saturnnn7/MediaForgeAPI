using MediaForge.Library.Application.Commands.AddWorkToList;
using MediaForge.Library.Application.Commands.CreateUserList;
using MediaForge.Library.Application.Commands.RemoveWorkFromList;
using MediaForge.Library.Application.Commands.UpdateUserList;
using MediaForge.Library.Application.Queries.GetList;
using MediaForge.Library.Application.Queries.GetMyLists;
using MediaForge.Library.Domain.Enums;

namespace MediaForge.Library.API.Endpoints;

public static class UserListEndpoints
{
    public static IEndpointRouteBuilder MapUserListEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/lists", async (CreateUserListRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateUserListCommand(request.Name, request.Slug, request.Description, request.AvatarUrl, request.Privacy);
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/lists/{result.Value.Id}", result.Value)
                : EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapPut("/api/lists/{listId:guid}", async (Guid listId, UpdateUserListRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateUserListCommand(listId, request.Name, request.Description, request.AvatarUrl, request.Privacy);
            var result = await sender.Send(command, ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapGet("/api/lists/my", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyListsQuery(), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapGet("/api/lists/{listId:guid}", async (Guid listId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetListQuery(listId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        app.MapPost("/api/lists/{listId:guid}/works", async (Guid listId, AddWorkToListRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new AddWorkToListCommand(listId, request.WorkId, request.DisplayOrder), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        app.MapDelete("/api/lists/{listId:guid}/works/{workId:guid}", async (Guid listId, Guid workId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveWorkFromListCommand(listId, workId), ct);
            return EndpointResults.ToHttpResult(result);
        }).RequireAuthorization();

        return app;
    }

    private sealed record CreateUserListRequest(string Name, string Slug, string? Description, string? AvatarUrl, ListPrivacy Privacy);

    private sealed record UpdateUserListRequest(string Name, string? Description, string? AvatarUrl, ListPrivacy Privacy);

    private sealed record AddWorkToListRequest(Guid WorkId, int DisplayOrder = 0);
}
