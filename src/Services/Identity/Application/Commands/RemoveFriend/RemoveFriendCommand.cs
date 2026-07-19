namespace MediaForge.Identity.Application.Commands.RemoveFriend;

public sealed record RemoveFriendCommand(Guid FriendId) : IRequest<Result>;
