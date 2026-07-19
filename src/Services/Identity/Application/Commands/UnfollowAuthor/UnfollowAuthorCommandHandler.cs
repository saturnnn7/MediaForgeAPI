using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.UnfollowAuthor;

public sealed class UnfollowAuthorCommandHandler(
    ICurrentUserService currentUser,
    IAuthorFollowRepository authorFollowRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<UnfollowAuthorCommand, Result>
{
    public async Task<Result> Handle(UnfollowAuthorCommand request, CancellationToken cancellationToken)
    {
        await authorFollowRepository.RemoveAsync(currentUser.UserId, request.PersonId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
