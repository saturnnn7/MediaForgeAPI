using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetPendingWorkRequests;

public sealed class GetPendingWorkRequestsQueryHandler(
    IWorkRequestRepository workRequestRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetPendingWorkRequestsQuery, Result<IReadOnlyList<WorkRequestDto>>>
{
    public async Task<Result<IReadOnlyList<WorkRequestDto>>> Handle(GetPendingWorkRequestsQuery request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
            return Result.Failure<IReadOnlyList<WorkRequestDto>>(Error.Unauthorized("Only an admin can view pending work requests."));

        var requests = await workRequestRepository.GetPendingAsync(cancellationToken);
        return Result.Success<IReadOnlyList<WorkRequestDto>>(requests.Select(r => r.ToDto()).ToList());
    }
}
