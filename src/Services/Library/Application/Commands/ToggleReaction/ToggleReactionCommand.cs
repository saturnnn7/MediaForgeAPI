namespace MediaForge.Library.Application.Commands.ToggleReaction;

public sealed record ToggleReactionCommand(Guid TargetId, ReactionTarget TargetType, string Emoji) : IRequest<Result>;
