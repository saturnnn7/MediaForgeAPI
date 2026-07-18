using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreatePart;

public sealed class CreatePartCommandHandler(
    IPartRepository partRepository,
    IWorkRepository workRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<CreatePartCommand, Result<PartSummaryDto>>
{
    public async Task<Result<PartSummaryDto>> Handle(CreatePartCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<PartSummaryDto>(Error.NotFound("Work", request.WorkId));

        var partResult = Part.Create(request.WorkId, request.Title, request.OrderMajor, request.OrderMinor, request.PartType);
        if (partResult.IsFailure)
            return Result.Failure<PartSummaryDto>(partResult.Error);

        var part = partResult.Value;

        if (request.Description is not null || request.CoverUrl is not null)
        {
            var updateResult = part.UpdateDetails(request.Title, request.Description, request.CoverUrl);
            if (updateResult.IsFailure)
                return Result.Failure<PartSummaryDto>(updateResult.Error);
        }

        await partRepository.AddAsync(part, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(part.ToSummaryDto());
    }
}
