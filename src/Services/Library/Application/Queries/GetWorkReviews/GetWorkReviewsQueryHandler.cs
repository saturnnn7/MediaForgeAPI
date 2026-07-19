using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetWorkReviews;

public sealed class GetWorkReviewsQueryHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetWorkReviewsQuery, Result<IReadOnlyList<ReviewDto>>>
{
    public async Task<Result<IReadOnlyList<ReviewDto>>> Handle(GetWorkReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await reviewRepository.GetByWorkIdAsync(request.WorkId, request.Page, request.PageSize, cancellationToken);

        var dtos = new List<ReviewDto>();
        foreach (var review in reviews)
        {
            var reactions = await reviewRepository.GetReactionsAsync(review.Id, cancellationToken);
            dtos.Add(review.ToDto(reactions, currentUserService.UserId));
        }

        return Result.Success<IReadOnlyList<ReviewDto>>(dtos);
    }
}
