using MediaForge.Identity.Application.Commands.MarkAllNotificationsRead;
using MediaForge.Identity.Application.Commands.MarkNotificationRead;
using MediaForge.Identity.Application.Queries.GetNotifications;
using MediaForge.Identity.Application.Queries.GetUnreadNotificationCount;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaForge.Identity.API.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/notifications")
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser());

        group.MapGet("/", async (int page, int pageSize, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetNotificationsQuery(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/unread-count", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetUnreadNotificationCountQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/{id:guid}/read", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new MarkNotificationReadCommand(id), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/read-all", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new MarkAllNotificationsReadCommand(), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.BadRequest(result.Error);
        });

        return app;
    }
}
