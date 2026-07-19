using MediaForge.Identity.Application.Commands.RemoveFriend;
using MediaForge.Identity.Application.Commands.RespondToFriendRequest;
using MediaForge.Identity.Application.Commands.SendFriendRequest;
using MediaForge.Identity.Application.Queries.GetFriends;
using MediaForge.Identity.Application.Queries.GetPendingRequests;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaForge.Identity.API.Endpoints;

public static class FriendEndpoints
{
    public static IEndpointRouteBuilder MapFriendEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/friends")
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser());

        group.MapPost("/request/{addresseeId:guid}", async (Guid addresseeId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SendFriendRequestCommand(addresseeId), ct);
            return result.IsSuccess
                ? Results.Created($"/api/friends/{result.Value.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/{friendshipId:guid}/accept", async (Guid friendshipId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RespondToFriendRequestCommand(friendshipId, true), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/{friendshipId:guid}/decline", async (Guid friendshipId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RespondToFriendRequestCommand(friendshipId, false), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapDelete("/{friendId:guid}", async (Guid friendId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveFriendCommand(friendId), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetFriendsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/pending", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetPendingRequestsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        return app;
    }
}
