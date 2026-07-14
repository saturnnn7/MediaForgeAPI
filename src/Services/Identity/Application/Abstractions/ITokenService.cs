using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Abstractions;

public interface ITokenService
{
    Task<TokensDto> GenerateTokensAsync(ApplicationUser user, CancellationToken ct);
    Task<Guid?> ValidateRefreshTokenAsync(string refreshToken, CancellationToken ct);
    Task<bool> ValidateEmailVerificationTokenAsync(Guid userId, string token, CancellationToken ct);
    Task<string> GenerateEmailVerificationTokenAsync(Guid userId, CancellationToken ct);
}
