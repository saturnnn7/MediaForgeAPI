using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.BecomeCreator;

public sealed class BecomeCreatorCommandHandler(
    ICurrentUserService currentUser,
    IApplicationUserRepository userRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<BecomeCreatorCommand, Result>
{
    public async Task<Result> Handle(BecomeCreatorCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("User", currentUser.UserId));
        }

        var result = user.BecomeCreator();
        if (result.IsFailure)
        {
            return result;
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
