using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.RespondToFriendRequest;

public sealed record RespondToFriendRequestCommand(Guid FriendshipId, bool Accept) : IRequest<Result<FriendshipDto>>;
