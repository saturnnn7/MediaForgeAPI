using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.RejectWorkRequest;

public sealed class RejectWorkRequestCommandHandler(
    IWorkRequestRepository workRequestRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<RejectWorkRequestCommand, Result<WorkRequestDto>>
{
    public async Task<Result<WorkRequestDto>> Handle(RejectWorkRequestCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
            return Result.Failure<WorkRequestDto>(Error.Unauthorized("Only an admin can reject work requests."));

        var workRequest = await workRequestRepository.GetByIdAsync(request.RequestId, cancellationToken);
        if (workRequest is null)
            return Result.Failure<WorkRequestDto>(Error.NotFound("WorkRequest", request.RequestId));

        var rejectResult = workRequest.Reject(request.AdminNote);
        if (rejectResult.IsFailure)
            return Result.Failure<WorkRequestDto>(rejectResult.Error);

        workRequestRepository.Update(workRequest);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(workRequest.ToDto());
    }
}
