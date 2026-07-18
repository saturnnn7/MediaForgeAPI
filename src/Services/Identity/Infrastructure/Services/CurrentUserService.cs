using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MediaForge.Identity.Infrastructure.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User
                ?? throw new InvalidOperationException("No HTTP context available.");

            var value = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException("User id claim not found.");

            return Guid.Parse(value);
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

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
