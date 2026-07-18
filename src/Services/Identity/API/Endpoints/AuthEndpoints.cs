using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;

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

        group.MapGet("/external-login", (string provider) =>
        {
            var scheme = provider.Equals("google", StringComparison.OrdinalIgnoreCase)
                ? GoogleDefaults.AuthenticationScheme
                : provider;

            var properties = new AuthenticationProperties { RedirectUri = "/api/auth/external-callback" };
            return Results.Challenge(properties, [scheme]);
        });

        group.MapGet("/external-callback", async (HttpContext httpContext, ISender sender, CancellationToken ct) =>
        {
            var authenticateResult = await httpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!authenticateResult.Succeeded || authenticateResult.Principal is null)
                return Results.Unauthorized();

            var principal = authenticateResult.Principal;
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var googleId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var displayName = principal.FindFirstValue(ClaimTypes.Name);
            var photoUrl = principal.FindFirstValue("picture");

            await httpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (email is null || googleId is null)
                return Results.Unauthorized();

            var result = await sender.Send(
                new FindOrCreateByGoogleCommand(email, googleId, displayName ?? email, photoUrl),
                ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
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

        group.MapPost("/become-creator", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new BecomeCreatorCommand(), ct);
            return result.IsSuccess
                ? Results.Ok()
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        group.MapGet("/profile", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetUserProfileQuery(), ct);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequireAuthorization(policy => policy
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser());

        return app;
    }
}
