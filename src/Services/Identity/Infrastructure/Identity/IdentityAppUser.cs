using Microsoft.AspNetCore.Identity;

namespace MediaForge.Identity.Infrastructure.Identity;

public sealed class IdentityAppUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool IsEmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
}
