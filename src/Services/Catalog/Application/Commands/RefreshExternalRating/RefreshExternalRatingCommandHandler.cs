using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.RefreshExternalRating;

public sealed class RefreshExternalRatingCommandHandler(
    IExternalRatingRepository externalRatingRepository,
    IExternalRatingFetcher externalRatingFetcher,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<RefreshExternalRatingCommand, Result<ExternalRatingDto>>
{
    public async Task<Result<ExternalRatingDto>> Handle(RefreshExternalRatingCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
            return Result.Failure<ExternalRatingDto>(Error.Unauthorized("Only an admin can refresh external ratings."));

        var rating = await externalRatingRepository.GetByWorkAndSourceAsync(request.WorkId, request.Source, cancellationToken);
        if (rating is null)
            return Result.Failure<ExternalRatingDto>(Error.NotFound("ExternalRating", request.WorkId));

        var fetched = await externalRatingFetcher.FetchAsync(request.Source, rating.ExternalId, cancellationToken);
        if (fetched is not null)
            rating.UpdateScore(fetched.Value.Score, fetched.Value.ReviewCount);

        externalRatingRepository.Update(rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(rating.ToDto());
    }
}
