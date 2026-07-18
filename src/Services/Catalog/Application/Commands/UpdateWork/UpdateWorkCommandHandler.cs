using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateWork;

public sealed class UpdateWorkCommandHandler(
    IWorkRepository workRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UpdateWorkCommand, Result<WorkSummaryDto>>
{
    public async Task<Result<WorkSummaryDto>> Handle(UpdateWorkCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure<WorkSummaryDto>(Error.NotFound("Work", request.WorkId));

        var updateResult = work.UpdateDetails(request.Title, request.Description, request.CoverUrl, request.Language);
        if (updateResult.IsFailure)
            return Result.Failure<WorkSummaryDto>(updateResult.Error);

        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(work.ToSummaryDto());
    }
}
