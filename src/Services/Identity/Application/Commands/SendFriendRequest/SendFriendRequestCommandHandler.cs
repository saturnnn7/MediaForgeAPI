using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.SendFriendRequest;

public sealed class SendFriendRequestCommandHandler(
    ICurrentUserService currentUser,
    IFriendshipRepository friendshipRepository,
    IApplicationUserRepository userRepository,
    INotificationRepository notificationRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<SendFriendRequestCommand, Result<FriendshipDto>>
{
    public async Task<Result<FriendshipDto>> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var existing = await friendshipRepository.GetBetweenUsersAsync(currentUser.UserId, request.AddresseeId, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<FriendshipDto>(Error.Conflict("Friendship", "A friend request already exists between these users."));
        }

        var friendshipResult = Friendship.Create(currentUser.UserId, request.AddresseeId);
        if (friendshipResult.IsFailure)
        {
            return Result.Failure<FriendshipDto>(friendshipResult.Error);
        }

        var friendship = friendshipResult.Value;
        await friendshipRepository.AddAsync(friendship, cancellationToken);

        var requester = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken);
        var notification = Notification.Create(
            request.AddresseeId,
            NotificationType.FriendRequestReceived,
            new { fromUserId = currentUser.UserId, fromDisplayName = requester?.DisplayName ?? string.Empty });
        await notificationRepository.AddAsync(notification, cancellationToken);

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
