using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.CreateReview;

public sealed class CreateReviewCommandHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<CreateReviewCommand, Result<ReviewDto>>
{
    public async Task<Result<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var existing = await reviewRepository.GetByUserAndWorkAsync(currentUserService.UserId, request.WorkId, cancellationToken);
        if (existing is not null)
            return Result.Failure<ReviewDto>(Error.Conflict("Review", "You have already reviewed this work."));

        var reviewResult = Review.Create(currentUserService.UserId, request.WorkId, request.Text, request.ContainsSpoiler);
        if (reviewResult.IsFailure)
            return Result.Failure<ReviewDto>(reviewResult.Error);

        var review = reviewResult.Value;

        await reviewRepository.AddAsync(review, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(review.ToDto([], currentUserService.UserId));
    }
}
