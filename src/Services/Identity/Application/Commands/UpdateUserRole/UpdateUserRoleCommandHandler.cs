using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.UpdateUserRole;

public sealed class UpdateUserRoleCommandHandler(
    IApplicationUserRepository userRepository,
    ICurrentUserService currentUserService,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<UpdateUserRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure(Error.Unauthorized("Only an admin can change user roles."));
        }

        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(Error.NotFound("User", request.UserId));
        }

        var result = user.ChangeRole(request.Role);
        if (result.IsFailure)
        {
            return result;
        }

        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
