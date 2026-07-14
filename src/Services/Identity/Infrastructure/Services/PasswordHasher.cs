using Microsoft.AspNetCore.Identity;
using MediaForge.Identity.Infrastructure.Identity;

namespace MediaForge.Identity.Infrastructure.Services;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<IdentityAppUser> _hasher = new();

    public string Hash(string password) =>
        _hasher.HashPassword(null!, password);

    public bool Verify(string password, string hash) =>
        _hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
}
