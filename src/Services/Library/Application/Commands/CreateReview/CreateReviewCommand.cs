using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.CreateReview;

public sealed record CreateReviewCommand(Guid WorkId, string Text, bool ContainsSpoiler) : IRequest<Result<ReviewDto>>;
