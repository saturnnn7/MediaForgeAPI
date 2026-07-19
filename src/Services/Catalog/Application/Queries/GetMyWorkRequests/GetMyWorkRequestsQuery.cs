using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetMyWorkRequests;

public sealed record GetMyWorkRequestsQuery : IRequest<Result<IReadOnlyList<WorkRequestDto>>>;
