using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MediaForge.Identity.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (RegisterCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/users/{result.Value.Id}", result.Value)
                : Results.BadRequest(result.Error);
        });

        group.MapPost("/login", async (LoginCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        });

        group.MapPost("/refresh", async (RefreshTokenCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        });

        group.MapGet("/verify-email", async (Guid userId, string token, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new VerifyEmailCommand(userId, token), ct);
            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(result.Error);
        });

        group.MapGet("/me/verification-status", async (ClaimsPrincipal user, IApplicationUserRepository userRepository, CancellationToken ct) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")!);
            var applicationUser = await userRepository.GetByIdAsync(userId, ct);

            return applicationUser is null
                ? Results.NotFound()
                : Results.Ok(new { isVerified = applicationUser.IsEmailVerified, email = applicationUser.Email });
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        return app;
    }
}
