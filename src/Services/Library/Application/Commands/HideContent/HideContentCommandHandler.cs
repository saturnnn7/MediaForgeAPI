using MediaForge.Library.Application.Abstractions;
using MediaForge.Library.Domain.Enums;

namespace MediaForge.Library.Application.Commands.HideContent;

public sealed class HideContentCommandHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<HideContentCommand, Result>
{
    public async Task<Result> Handle(HideContentCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure(Error.Unauthorized("Only an admin can hide content."));
        }

        if (request.TargetType == ReactionTarget.Review)
        {
            var review = await reviewRepository.GetByIdAsync(request.TargetId, cancellationToken);
            if (review is null)
            {
                return Result.Failure(Error.NotFound("Review", request.TargetId));
            }

            review.Hide();
            reviewRepository.Update(review);
        }
        else
        {
            var comment = await reviewRepository.GetCommentByIdAsync(request.TargetId, cancellationToken);
            if (comment is null)
            {
                return Result.Failure(Error.NotFound("ReviewComment", request.TargetId));
            }

            comment.Hide();
            reviewRepository.UpdateComment(comment);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
