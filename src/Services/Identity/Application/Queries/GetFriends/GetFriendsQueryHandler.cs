using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetFriends;

public sealed class GetFriendsQueryHandler(
    ICurrentUserService currentUser,
    IFriendshipRepository friendshipRepository) : IRequestHandler<GetFriendsQuery, Result<IReadOnlyList<FriendshipDto>>>
{
    public async Task<Result<IReadOnlyList<FriendshipDto>>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var friends = await friendshipRepository.GetFriendsAsync(currentUser.UserId, cancellationToken);

        IReadOnlyList<FriendshipDto> dtos = friends
            .Select(f => new FriendshipDto(f.Id, f.RequesterId, f.AddresseeId, f.Status.ToString(), f.CreatedAt, f.RespondedAt))
            .ToList();

        return Result.Success(dtos);
    }
}
