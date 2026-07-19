using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.EditReview;

public sealed record EditReviewCommand(Guid ReviewId, string Text, bool ContainsSpoiler) : IRequest<Result<ReviewDto>>;
