namespace MediaForge.Library.Application.Commands.ReportContent;

public sealed record ReportContentCommand(Guid TargetId, ReactionTarget TargetType, string Reason) : IRequest<Result>;
