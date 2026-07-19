using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetFriends;

public sealed record GetFriendsQuery : IRequest<Result<IReadOnlyList<FriendshipDto>>>;
