using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.AddComment;

public sealed class AddCommentCommandHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<AddCommentCommand, Result<ReviewCommentDto>>
{
    public async Task<Result<ReviewCommentDto>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review is null)
            return Result.Failure<ReviewCommentDto>(Error.NotFound("Review", request.ReviewId));

        var depth = 0;
        if (request.ParentCommentId is { } parentId)
        {
            var parentComment = await reviewRepository.GetCommentByIdAsync(parentId, cancellationToken);
            if (parentComment is null)
                return Result.Failure<ReviewCommentDto>(Error.NotFound("ReviewComment", parentId));

            depth = parentComment.Depth + 1;
        }

        var commentResult = ReviewComment.Create(request.ReviewId, request.ParentCommentId, currentUserService.UserId, request.Text, request.ContainsSpoiler, depth);
        if (commentResult.IsFailure)
            return Result.Failure<ReviewCommentDto>(commentResult.Error);

        var comment = commentResult.Value;

        await reviewRepository.AddCommentAsync(comment, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(comment.ToDto([], currentUserService.UserId));
    }
}
