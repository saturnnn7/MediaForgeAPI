using MediaForge.Identity.Application.Commands.FollowAuthor;
using MediaForge.Identity.Application.Commands.UnfollowAuthor;
using MediaForge.Identity.Application.Queries.GetMyFollows;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaForge.Identity.API.Endpoints;

public static class AuthorFollowEndpoints
{
    public static IEndpointRouteBuilder MapAuthorFollowEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/authors")
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser());

        group.MapPost("/{personId:guid}/follow", async (Guid personId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new FollowAuthorCommand(personId), ct);
            return result.IsSuccess
                ? Results.Created($"/api/authors/{personId}/follow", result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapDelete("/{personId:guid}/follow", async (Guid personId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UnfollowAuthorCommand(personId), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/following", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyFollowsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        return app;
    }
}
