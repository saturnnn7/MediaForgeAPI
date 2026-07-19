using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.AddComment;

public sealed record AddCommentCommand(Guid ReviewId, Guid? ParentCommentId, string Text, bool ContainsSpoiler) : IRequest<Result<ReviewCommentDto>>;
