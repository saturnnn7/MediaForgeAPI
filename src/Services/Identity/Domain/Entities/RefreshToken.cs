namespace MediaForge.Identity.Domain.Entities;

public sealed class RefreshToken
{
    private RefreshToken() { }

    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; init; }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke() => IsRevoked = true;

    public static RefreshToken Create(string token, DateTime expiresAt) =>
        new()
        {
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow
        };
}
