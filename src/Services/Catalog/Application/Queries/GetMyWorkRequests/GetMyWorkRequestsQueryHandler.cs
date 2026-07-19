using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetMyWorkRequests;

public sealed class GetMyWorkRequestsQueryHandler(
    IWorkRequestRepository workRequestRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyWorkRequestsQuery, Result<IReadOnlyList<WorkRequestDto>>>
{
    public async Task<Result<IReadOnlyList<WorkRequestDto>>> Handle(GetMyWorkRequestsQuery request, CancellationToken cancellationToken)
    {
        var requests = await workRequestRepository.GetByRequesterIdAsync(currentUserService.UserId, cancellationToken);
        return Result.Success<IReadOnlyList<WorkRequestDto>>(requests.Select(r => r.ToDto()).ToList());
    }
}
