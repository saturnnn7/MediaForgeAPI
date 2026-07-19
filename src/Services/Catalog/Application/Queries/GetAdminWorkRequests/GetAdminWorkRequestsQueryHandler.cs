using MediaForge.Catalog.Application.Abstractions;
using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetAdminWorkRequests;

public sealed class GetAdminWorkRequestsQueryHandler(
    IWorkRequestRepository workRequestRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetAdminWorkRequestsQuery, Result<IReadOnlyList<WorkRequestDto>>>
{
    public async Task<Result<IReadOnlyList<WorkRequestDto>>> Handle(GetAdminWorkRequestsQuery request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure<IReadOnlyList<WorkRequestDto>>(Error.Unauthorized("Only an admin can list work requests."));
        }

        var requests = await workRequestRepository.GetAllAsync(request.Status, request.Page, request.PageSize, cancellationToken);
        return Result.Success<IReadOnlyList<WorkRequestDto>>(requests.Select(r => r.ToDto()).ToList());
    }
}
