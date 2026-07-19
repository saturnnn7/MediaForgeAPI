using MediaForge.Library.Domain.Enums;

namespace MediaForge.Library.Application.Commands.HideContent;

public sealed record HideContentCommand(Guid TargetId, ReactionTarget TargetType) : IRequest<Result>;
