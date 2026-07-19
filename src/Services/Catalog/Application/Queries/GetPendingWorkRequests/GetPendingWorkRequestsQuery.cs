using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetPendingWorkRequests;

public sealed record GetPendingWorkRequestsQuery : IRequest<Result<IReadOnlyList<WorkRequestDto>>>;
