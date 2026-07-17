using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using MediaForge.Identity.Application.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Identity.Infrastructure.Services;

public sealed class TokenService(IConfiguration configuration, IDistributedCache cache) : ITokenService
{
    private const string RefreshTokenKeyPrefix = "rt:";
    private const string EmailVerificationTokenKeyPrefix = "evt:";
    private const string VerificationTokenAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public async Task<TokensDto> GenerateTokensAsync(ApplicationUser user, CancellationToken ct)
    {
        var accessTokenExpiryMinutes = int.Parse(configuration["Jwt:AccessTokenExpiryMinutes"] ?? "15", CultureInfo.InvariantCulture);
        var refreshTokenExpiryDays = int.Parse(configuration["Jwt:RefreshTokenExpiryDays"] ?? "7", CultureInfo.InvariantCulture);
        var expiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes);

        var accessToken = CreateAccessToken(user, expiresAt);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        await cache.SetStringAsync(
            $"{RefreshTokenKeyPrefix}{refreshToken}",
            user.Id.ToString(),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(refreshTokenExpiryDays) },
            ct);

        return new TokensDto(accessToken, refreshToken, expiresAt);
    }

    public async Task<Guid?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        var userId = await cache.GetStringAsync($"{RefreshTokenKeyPrefix}{refreshToken}", ct);
        if (userId is null)
        {
            return null;
        }

        return Guid.Parse(userId);
    }

    public async Task<bool> ValidateEmailVerificationTokenAsync(Guid userId, string token, CancellationToken ct)
    {
        var key = $"{EmailVerificationTokenKeyPrefix}{userId}:{token}";
        var value = await cache.GetStringAsync(key, ct);
        if (value is null)
        {
            return false;
        }

        await cache.RemoveAsync(key, ct);
        return true;
    }

    public async Task<string> GenerateEmailVerificationTokenAsync(Guid userId, CancellationToken ct)
    {
        var token = RandomString(6);

        await cache.SetStringAsync(
            $"{EmailVerificationTokenKeyPrefix}{userId}:{token}",
            "1",
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) },
            ct);

        return token;
    }

    private static RSA? _signingKey;

    private string CreateAccessToken(ApplicationUser user, DateTime expiresAt)
    {
        var rsa = LazyInitializer.EnsureInitialized(ref _signingKey, () =>
        {
            var privateKeyPath = configuration["Jwt:PrivateKeyPath"]!;
            var key = RSA.Create();
            key.ImportFromPem(File.ReadAllText(privateKeyPath));
            return key;
        });

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("display_name", user.DisplayName),
            new Claim("email_verified", user.IsEmailVerified.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string RandomString(int length)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        var builder = new StringBuilder(length);
        foreach (var b in bytes)
        {
            builder.Append(VerificationTokenAlphabet[b % VerificationTokenAlphabet.Length]);
        }

        return builder.ToString();
    }
}
