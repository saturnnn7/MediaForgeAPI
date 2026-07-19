using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkExternalRatings;

public sealed record GetWorkExternalRatingsQuery(Guid WorkId) : IRequest<Result<IReadOnlyList<ExternalRatingDto>>>;
