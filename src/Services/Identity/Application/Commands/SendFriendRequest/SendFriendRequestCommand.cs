using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.SendFriendRequest;

public sealed record SendFriendRequestCommand(Guid AddresseeId) : IRequest<Result<FriendshipDto>>;
