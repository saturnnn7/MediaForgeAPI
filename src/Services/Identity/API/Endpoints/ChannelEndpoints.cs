using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaForge.Identity.API.Endpoints;

public static class ChannelEndpoints
{
    public static IEndpointRouteBuilder MapChannelEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/channels");

        group.MapPost("/", async (CreateChannelCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/channels/{result.Value.Id}", result.Value)
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        group.MapPut("/me", async (UpdateChannelCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        group.MapGet("/me", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMyChannelQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        group.MapGet("/subscriptions", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetMySubscriptionsQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        group.MapGet("/{channelId:guid}", async (Guid channelId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetChannelQuery(channelId), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.NotFound(result.Error);
        });

        group.MapPost("/{channelId:guid}/subscribe", async (Guid channelId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new SubscribeToChannelCommand(channelId), ct);
            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        group.MapDelete("/{channelId:guid}/subscribe", async (Guid channelId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new UnsubscribeFromChannelCommand(channelId), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        return app;
    }
}
