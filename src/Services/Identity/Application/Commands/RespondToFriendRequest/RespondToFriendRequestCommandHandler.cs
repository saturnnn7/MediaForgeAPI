using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.RespondToFriendRequest;

public sealed class RespondToFriendRequestCommandHandler(
    ICurrentUserService currentUser,
    IFriendshipRepository friendshipRepository,
    INotificationRepository notificationRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<RespondToFriendRequestCommand, Result<FriendshipDto>>
{
    public async Task<Result<FriendshipDto>> Handle(RespondToFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var friendship = await friendshipRepository.GetByIdAsync(request.FriendshipId, cancellationToken);
        if (friendship is null)
        {
            return Result.Failure<FriendshipDto>(Error.NotFound("Friendship", request.FriendshipId));
        }

        if (friendship.AddresseeId != currentUser.UserId)
        {
            return Result.Failure<FriendshipDto>(Error.Unauthorized("You cannot respond to this friend request."));
        }

        var respondResult = request.Accept ? friendship.Accept() : friendship.Decline();
        if (respondResult.IsFailure)
        {
            return Result.Failure<FriendshipDto>(respondResult.Error);
        }

        if (request.Accept)
        {
            var notification = Notification.Create(
                friendship.RequesterId,
                NotificationType.FriendRequestAccepted,
                new { byUserId = currentUser.UserId });
            await notificationRepository.AddAsync(notification, cancellationToken);
        }

        friendshipRepository.Update(friendship);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new FriendshipDto(
            friendship.Id,
            friendship.RequesterId,
            friendship.AddresseeId,
            friendship.Status.ToString(),
            friendship.CreatedAt,
            friendship.RespondedAt));
    }
}
