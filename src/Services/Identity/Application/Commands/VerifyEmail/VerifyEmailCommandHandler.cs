using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.VerifyEmail;

public sealed class VerifyEmailCommandHandler(
    IApplicationUserRepository userRepository,
    ITokenService tokenService,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<VerifyEmailCommand, Result>
{
    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(Error.NotFound("User", request.UserId));

        var isValidToken = await tokenService.ValidateEmailVerificationTokenAsync(request.UserId, request.Token, cancellationToken);
        if (!isValidToken)
            return Result.Failure(Error.Validation("Token", "Invalid or expired verification token."));

        var result = user.VerifyEmail();
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
