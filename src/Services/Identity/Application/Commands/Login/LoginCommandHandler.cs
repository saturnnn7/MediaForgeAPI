using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.Login;

public sealed class LoginCommandHandler(
    IApplicationUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<LoginCommand, Result<TokensDto>>
{
    public async Task<Result<TokensDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
            return Result.Failure<TokensDto>(Error.Validation("Email", "Invalid credentials."));

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result.Failure<TokensDto>(Error.Validation("Password", "Invalid credentials."));

        var tokens = await tokenService.GenerateTokensAsync(user, cancellationToken);

        user.AddRefreshToken(global::MediaForge.Identity.Domain.Entities.RefreshToken.Create(tokens.RefreshToken, tokens.ExpiresAt));
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(tokens);
    }
}
