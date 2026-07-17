using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.RevokeCreator;

public sealed class RevokeCreatorCommandHandler(
    IApplicationUserRepository userRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<RevokeCreatorCommand, Result>
{
    public async Task<Result> Handle(RevokeCreatorCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("User", request.TargetUserId));
        }

        var result = user.RevokeCreator();
        if (result.IsFailure)
        {
            return result;
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
