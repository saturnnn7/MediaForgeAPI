using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.ApproveWorkRequest;

public sealed class ApproveWorkRequestCommandHandler(
    IWorkRequestRepository workRequestRepository,
    IWorkRepository workRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<ApproveWorkRequestCommand, Result<WorkRequestDto>>
{
    public async Task<Result<WorkRequestDto>> Handle(ApproveWorkRequestCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
            return Result.Failure<WorkRequestDto>(Error.Unauthorized("Only an admin can approve work requests."));

        var workRequest = await workRequestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (workRequest is null)
            return Result.Failure<WorkRequestDto>(Error.NotFound("WorkRequest", request.RequestId));

        var workResult = Work.Create(request.ChannelId, workRequest.RequesterId, workRequest.WorkType, workRequest.Title);
        if (workResult.IsFailure)
            return Result.Failure<WorkRequestDto>(workResult.Error);

        var work = workResult.Value;

        var updateResult = work.UpdateDetails(workRequest.Title, workRequest.Description, workRequest.CoverUrl, workRequest.Language);
        if (updateResult.IsFailure)
            return Result.Failure<WorkRequestDto>(updateResult.Error);

        // Note: AuthorNames (comma-separated) is not converted into Person/WorkContributor records here;
        // that remains a manual follow-up step for the admin after approval.

        await workRepository.AddAsync(work, cancellationToken);

        var approveResult = workRequest.Approve(work.Id);
        if (approveResult.IsFailure)
            return Result.Failure<WorkRequestDto>(approveResult.Error);

        workRequestRepository.Update(workRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(workRequest.ToDto());
    }
}
