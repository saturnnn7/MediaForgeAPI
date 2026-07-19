using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetPendingRequests;

public sealed record GetPendingRequestsQuery : IRequest<Result<IReadOnlyList<FriendshipDto>>>;
