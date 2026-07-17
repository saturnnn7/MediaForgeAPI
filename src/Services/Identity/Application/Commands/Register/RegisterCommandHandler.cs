using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.Register;

public sealed class RegisterCommandHandler(
    IApplicationUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            return Result.Failure<UserProfileDto>(Error.Conflict("User", "Email already registered."));
        }

        var user = ApplicationUser.Create(request.Email, request.DisplayName);
        user.SetPasswordHash(passwordHasher.Hash(request.Password));

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new UserProfileDto(
            user.Id,
            user.Email,
            user.DisplayName,
            user.AvatarUrl,
            user.IsEmailVerified,
            user.Role.ToString().ToLowerInvariant(),
            user.CreatedAt));
    }
}
