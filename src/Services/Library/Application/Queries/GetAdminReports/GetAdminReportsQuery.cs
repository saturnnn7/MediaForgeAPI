using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetAdminReports;

public sealed record GetAdminReportsQuery(int Page, int PageSize) : IRequest<Result<IReadOnlyList<AdminReportDto>>>;
