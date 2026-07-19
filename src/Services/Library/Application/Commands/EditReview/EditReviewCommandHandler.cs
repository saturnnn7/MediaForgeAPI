using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.EditReview;

public sealed class EditReviewCommandHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<EditReviewCommand, Result<ReviewDto>>
{
    public async Task<Result<ReviewDto>> Handle(EditReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdWithCommentsAsync(request.ReviewId, cancellationToken);
        if (review is null)
            return Result.Failure<ReviewDto>(Error.NotFound("Review", request.ReviewId));

        if (review.UserId != currentUserService.UserId)
            return Result.Failure<ReviewDto>(Error.Unauthorized("You do not own this review."));

        var editResult = review.Edit(request.Text, request.ContainsSpoiler);
        if (editResult.IsFailure)
            return Result.Failure<ReviewDto>(editResult.Error);

        reviewRepository.Update(review);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var reactions = await reviewRepository.GetReactionsAsync(review.Id, cancellationToken);

        return Result.Success(review.ToDto(reactions, currentUserService.UserId));
    }
}
