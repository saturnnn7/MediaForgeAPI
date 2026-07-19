using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.SubmitWorkRequest;

public sealed class SubmitWorkRequestCommandHandler(
    IWorkRequestRepository workRequestRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<SubmitWorkRequestCommand, Result<WorkRequestDto>>
{
    public async Task<Result<WorkRequestDto>> Handle(SubmitWorkRequestCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role is not ("creator" or "admin"))
            return Result.Failure<WorkRequestDto>(Error.Unauthorized("Only creators can submit work requests."));

        var requestResult = WorkRequest.Create(
            currentUserService.UserId,
            request.WorkType,
            request.Title,
            request.AuthorNames,
            request.Description,
            request.CoverUrl,
            request.Language);

        if (requestResult.IsFailure)
            return Result.Failure<WorkRequestDto>(requestResult.Error);

        var workRequest = requestResult.Value;

        await workRequestRepository.AddAsync(workRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(workRequest.ToDto());
    }
}
