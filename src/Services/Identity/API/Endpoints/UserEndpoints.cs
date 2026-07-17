using System.Security.Claims;
using MediaForge.Identity.Application.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaForge.Identity.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .RequireAuthorization(policy => policy
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser());

        group.MapGet("/me", async (ClaimsPrincipal user, IApplicationUserRepository userRepository, CancellationToken ct) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")!);
            var applicationUser = await userRepository.GetByIdAsync(userId, ct);

            if (applicationUser is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(new UserProfileDto(
                applicationUser.Id,
                applicationUser.Email,
                applicationUser.DisplayName,
                applicationUser.AvatarUrl,
                applicationUser.IsEmailVerified,
                applicationUser.Role.ToString().ToLowerInvariant(),
                applicationUser.CreatedAt));
        });

        return app;
    }
}
