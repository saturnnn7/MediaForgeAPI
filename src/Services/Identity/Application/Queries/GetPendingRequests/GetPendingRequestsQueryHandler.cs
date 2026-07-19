using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetPendingRequests;

public sealed class GetPendingRequestsQueryHandler(
    ICurrentUserService currentUser,
    IFriendshipRepository friendshipRepository) : IRequestHandler<GetPendingRequestsQuery, Result<IReadOnlyList<FriendshipDto>>>
{
    public async Task<Result<IReadOnlyList<FriendshipDto>>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
    {
        var pending = await friendshipRepository.GetPendingRequestsAsync(currentUser.UserId, cancellationToken);

        IReadOnlyList<FriendshipDto> dtos = pending
            .Select(f => new FriendshipDto(f.Id, f.RequesterId, f.AddresseeId, f.Status.ToString(), f.CreatedAt, f.RespondedAt))
            .ToList();

        return Result.Success(dtos);
    }
}
