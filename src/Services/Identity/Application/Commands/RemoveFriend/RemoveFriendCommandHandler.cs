using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.RemoveFriend;

public sealed class RemoveFriendCommandHandler(
    ICurrentUserService currentUser,
    IFriendshipRepository friendshipRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<RemoveFriendCommand, Result>
{
    public async Task<Result> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
    {
        var friendship = await friendshipRepository.GetBetweenUsersAsync(currentUser.UserId, request.FriendId, cancellationToken);
        if (friendship is null)
        {
            return Result.Failure(Error.NotFound("Friendship", request.FriendId));
        }

        await friendshipRepository.DeleteAsync(friendship, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
