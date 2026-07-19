using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.BanUser;

public sealed class BanUserCommandHandler(
    IApplicationUserRepository userRepository,
    ICurrentUserService currentUserService,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<BanUserCommand, Result>
{
    public async Task<Result> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure(Error.Unauthorized("Only an admin can ban or unban users."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("User", request.UserId));
        }

        var result = request.IsBanned ? user.Ban(request.Reason) : user.Unban();
        if (result.IsFailure)
        {
            return result;
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
