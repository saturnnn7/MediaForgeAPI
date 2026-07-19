using MediaForge.Library.Application.Abstractions;
using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetAdminReports;

public sealed class GetAdminReportsQueryHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetAdminReportsQuery, Result<IReadOnlyList<AdminReportDto>>>
{
    public async Task<Result<IReadOnlyList<AdminReportDto>>> Handle(GetAdminReportsQuery request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure<IReadOnlyList<AdminReportDto>>(Error.Unauthorized("Only an admin can list reports."));
        }

        var reports = await reviewRepository.GetReportsAsync(request.Page, request.PageSize, cancellationToken);

        return Result.Success<IReadOnlyList<AdminReportDto>>(reports.Select(r => new AdminReportDto(
            r.Id,
            r.TargetId,
            r.TargetType.ToString(),
            r.ReporterId,
            r.Reason,
            r.IsResolved,
            r.ResolvedAt,
            r.CreatedAt)).ToList());
    }
}
