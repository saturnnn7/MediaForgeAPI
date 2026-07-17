using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IApplicationUserRepository userRepository,
    ITokenService tokenService,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, Result<TokensDto>>
{
    public async Task<Result<TokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = await tokenService.ValidateRefreshTokenAsync(request.RefreshToken, cancellationToken);
        if (userId is null)
        {
            return Result.Failure<TokensDto>(Error.Validation("RefreshToken", "Invalid or expired."));
        }

        var user = await userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure<TokensDto>(Error.Validation("RefreshToken", "Invalid or expired."));
        }

        user.RevokeRefreshToken(request.RefreshToken);

        var tokens = await tokenService.GenerateTokensAsync(user, cancellationToken);
        user.AddRefreshToken(global::MediaForge.Identity.Domain.Entities.RefreshToken.Create(tokens.RefreshToken, tokens.ExpiresAt));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tokens);
    }
}
