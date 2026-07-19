using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkExternalRatings;

public sealed class GetWorkExternalRatingsQueryHandler(IExternalRatingRepository externalRatingRepository)
    : IRequestHandler<GetWorkExternalRatingsQuery, Result<IReadOnlyList<ExternalRatingDto>>>
{
    public async Task<Result<IReadOnlyList<ExternalRatingDto>>> Handle(GetWorkExternalRatingsQuery request, CancellationToken cancellationToken)
    {
        var ratings = await externalRatingRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
        IReadOnlyList<ExternalRatingDto> dtos = ratings.Select(x => x.ToDto()).ToList();

        return Result.Success(dtos);
    }
}
