using MediaForge.Identity.Application.Commands.BanUser;
using MediaForge.Identity.Application.Commands.UpdateUserRole;
using MediaForge.Identity.Application.Queries.GetAdminUserById;
using MediaForge.Identity.Application.Queries.GetAdminUsers;
using MediaForge.Identity.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaForge.Identity.API.Endpoints;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin/users")
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser());

        group.MapGet("/", async (int? page, int? pageSize, string? role, string? search, ISender sender, CancellationToken ct) =>
        {
            UserRole? roleFilter = role is not null && Enum.TryParse<UserRole>(role, true, out var parsed) ? parsed : null;
            var result = await sender.Send(new GetAdminUsersQuery(page ?? 1, pageSize ?? 20, roleFilter, search), ct);
            return EndpointResults.ToHttpResult(result);
        });

        group.MapGet("/{userId:guid}", async (Guid userId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAdminUserByIdQuery(userId), ct);
            return EndpointResults.ToHttpResult(result);
        });

        group.MapPatch("/{userId:guid}/role", async (Guid userId, UpdateUserRoleRequest request, ISender sender, CancellationToken ct) =>
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            {
                return Results.BadRequest(new { error = $"Unknown role '{request.Role}'." });
            }

            var result = await sender.Send(new UpdateUserRoleCommand(userId, role), ct);
            return EndpointResults.ToHttpResult(result);
        });

        group.MapPatch("/{userId:guid}/ban", async (Guid userId, BanUserRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new BanUserCommand(userId, request.IsBanned, request.Reason), ct);
            return EndpointResults.ToHttpResult(result);
        });

        return app;
    }
}

public sealed record UpdateUserRoleRequest(string Role);

public sealed record BanUserRequest(bool IsBanned, string? Reason);
