using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MediaForge.Identity.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue("sub")
                ?? httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(value, out var id) ? id : Guid.Empty;
        }
    }

    public string Email
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User
                ?? throw new InvalidOperationException("No HTTP context available.");

            return user.FindFirstValue("email") ?? user.FindFirstValue(ClaimTypes.Email)
                ?? throw new InvalidOperationException("Email claim not found.");
        }
    }

    public string? Role => httpContextAccessor.HttpContext?.User.FindFirstValue("role");

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
