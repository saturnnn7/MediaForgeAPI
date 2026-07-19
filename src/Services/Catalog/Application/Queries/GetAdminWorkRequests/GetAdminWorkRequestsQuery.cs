using MediaForge.Catalog.Application.DTOs;
using MediaForge.Catalog.Domain.Enums;

namespace MediaForge.Catalog.Application.Queries.GetAdminWorkRequests;

public sealed record GetAdminWorkRequestsQuery(WorkRequestStatus? Status, int Page, int PageSize) : IRequest<Result<IReadOnlyList<WorkRequestDto>>>;
