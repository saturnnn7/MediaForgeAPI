using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetWorkReviews;

public sealed record GetWorkReviewsQuery(Guid WorkId, int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<ReviewDto>>>;
