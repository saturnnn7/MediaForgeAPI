using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.FindOrCreateByGoogle;

public sealed class FindOrCreateByGoogleCommandHandler(
    IApplicationUserRepository userRepository,
    ITokenService tokenService,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<FindOrCreateByGoogleCommand, Result<TokensDto>>
{
    public async Task<Result<TokensDto>> Handle(FindOrCreateByGoogleCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            user = ApplicationUser.Create(request.Email, request.DisplayName);
            user.SetPasswordHash("GOOGLE_AUTH_NO_PASSWORD");
            user.VerifyEmail();

            if (request.PhotoUrl is not null)
            {
                user.UpdateAvatar(request.PhotoUrl);
            }

            await userRepository.AddAsync(user, cancellationToken);
        }

        var tokens = await tokenService.GenerateTokensAsync(user, cancellationToken);

        user.AddRefreshToken(global::MediaForge.Identity.Domain.Entities.RefreshToken.Create(tokens.RefreshToken, tokens.ExpiresAt));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tokens);
    }
}
