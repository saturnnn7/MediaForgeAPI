using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateEdition;

public sealed class UpdateEditionCommandHandler(
    IEditionRepository editionRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UpdateEditionCommand, Result<EditionDto>>
{
    public async Task<Result<EditionDto>> Handle(UpdateEditionCommand request, CancellationToken cancellationToken)
    {
        var edition = await editionRepository.GetByIdAsync(request.EditionId, cancellationToken);
        if (edition is null)
            return Result.Failure<EditionDto>(Error.NotFound("Edition", request.EditionId));

        if (edition.CreatorId != currentUserService.UserId && currentUserService.Role != "admin")
            return Result.Failure<EditionDto>(Error.Unauthorized("Only the edition owner can update this edition."));

        var updateResult = edition.UpdateDetails(request.NarratorTeamName, request.Description, request.CoverUrl, request.Language);
        if (updateResult.IsFailure)
            return Result.Failure<EditionDto>(updateResult.Error);

        editionRepository.Update(edition);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(edition.ToDto());
    }
}
