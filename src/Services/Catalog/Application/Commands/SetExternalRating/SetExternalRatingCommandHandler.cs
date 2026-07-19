using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.SetExternalRating;

public sealed class SetExternalRatingCommandHandler(
    IExternalRatingRepository externalRatingRepository,
    IExternalRatingFetcher externalRatingFetcher,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<SetExternalRatingCommand, Result<ExternalRatingDto>>
{
    public async Task<Result<ExternalRatingDto>> Handle(SetExternalRatingCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
            return Result.Failure<ExternalRatingDto>(Error.Unauthorized("Only an admin can set external ratings."));

        var rating = await externalRatingRepository.GetByWorkAndSourceAsync(request.WorkId, request.Source, cancellationToken);
        if (rating is null)
        {
            var createResult = ExternalRating.Create(request.WorkId, request.Source, request.ExternalId, request.ExternalUrl);
            if (createResult.IsFailure)
                return Result.Failure<ExternalRatingDto>(createResult.Error);

            rating = createResult.Value;
            await externalRatingRepository.AddAsync(rating, cancellationToken);
        }
        else
        {
            var updateResult = rating.UpdateExternalId(request.ExternalId, request.ExternalUrl);
            if (updateResult.IsFailure)
                return Result.Failure<ExternalRatingDto>(updateResult.Error);

            externalRatingRepository.Update(rating);
        }

        var fetched = await externalRatingFetcher.FetchAsync(request.Source, request.ExternalId, cancellationToken);
        if (fetched is not null)
            rating.UpdateScore(fetched.Value.Score, fetched.Value.ReviewCount);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(rating.ToDto());
    }
}
