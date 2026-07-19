using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdatePart;

public sealed class UpdatePartCommandHandler(
    IPartRepository partRepository,
    IEditionRepository editionRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UpdatePartCommand, Result<PartSummaryDto>>
{
    public async Task<Result<PartSummaryDto>> Handle(UpdatePartCommand request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdAsync(request.PartId, cancellationToken);
        if (part is null)
            return Result.Failure<PartSummaryDto>(Error.NotFound("Part", request.PartId));

        var updateResult = part.UpdateDetails(request.Title, request.Description, request.CoverUrl);
        if (updateResult.IsFailure)
            return Result.Failure<PartSummaryDto>(updateResult.Error);

        partRepository.Update(part);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var edition = await editionRepository.GetByIdAsync(part.EditionId, cancellationToken);

        return Result.Success(part.ToSummaryDto(edition?.NarratorTeamName));
    }
}
