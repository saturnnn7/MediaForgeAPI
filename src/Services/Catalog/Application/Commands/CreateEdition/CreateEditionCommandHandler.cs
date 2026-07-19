using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateEdition;

public sealed class CreateEditionCommandHandler(
    IEditionRepository editionRepository,
    IWorkRepository workRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<CreateEditionCommand, Result<EditionDto>>
{
    public async Task<Result<EditionDto>> Handle(CreateEditionCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<EditionDto>(Error.NotFound("Work", request.WorkId));

        if (currentUserService.Role is not ("creator" or "admin"))
            return Result.Failure<EditionDto>(Error.Unauthorized("Only creators can add editions."));

        var editionResult = Edition.Create(request.WorkId, currentUserService.UserId, request.NarratorTeamName, request.Language);
        if (editionResult.IsFailure)
            return Result.Failure<EditionDto>(editionResult.Error);

        var edition = editionResult.Value;

        if (request.Description is not null || request.CoverUrl is not null)
        {
            var updateResult = edition.UpdateDetails(request.NarratorTeamName, request.Description, request.CoverUrl, request.Language);
            if (updateResult.IsFailure)
                return Result.Failure<EditionDto>(updateResult.Error);
        }

        var existingEditions = await editionRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
        if (existingEditions.Count == 0)
            edition.SetAsDefault();

        await editionRepository.AddAsync(edition, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(edition.ToDto());
    }
}
