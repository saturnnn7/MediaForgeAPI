using MediaForge.Identity.Domain.Events;

namespace MediaForge.Identity.Domain.Entities;

public sealed class ApplicationUser : AggregateRoot
{
    private readonly List<RefreshToken> _refreshTokens = [];

    private ApplicationUser() { }

    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public UserRole Role { get; private set; } = UserRole.Listener;
    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public static ApplicationUser Create(string email, string displayName)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = displayName,
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, user.Email, user.DisplayName));

        return user;
    }

    public void SetPasswordHash(string hash) => PasswordHash = hash;

    public Result VerifyEmail()
    {
        if (IsEmailVerified)
        {
            return Result.Failure(Error.Conflict("User", "Email is already verified."));
        }

        IsEmailVerified = true;
        RaiseDomainEvent(new UserEmailVerifiedDomainEvent(Id, Email));

        return Result.Success();
    }

    public Result AddRefreshToken(RefreshToken token)
    {
        _refreshTokens.Add(token);
        return Result.Success();
    }

    public Result RevokeRefreshToken(string token)
    {
        var refreshToken = _refreshTokens.SingleOrDefault(t => t.Token == token);
        if (refreshToken is null)
        {
            return Result.Failure(Error.Validation("Token", "Refresh token not found."));
        }

        refreshToken.Revoke();
        return Result.Success();
    }

    public Result UpdateAvatar(string url)
    {
        AvatarUrl = url;
        return Result.Success();
    }

    public Result BecomeCreator()
    {
        if (Role == UserRole.Creator)
        {
            return Result.Failure(Error.Conflict("User", "Already a Creator."));
        }

        if (!IsEmailVerified)
        {
            return Result.Failure(Error.Validation("Email", "Email must be verified to become a Creator."));
        }

        Role = UserRole.Creator;
        return Result.Success();
    }

    public Result RevokeCreator()
    {
        Role = UserRole.Listener;
        return Result.Success();
    }
}
