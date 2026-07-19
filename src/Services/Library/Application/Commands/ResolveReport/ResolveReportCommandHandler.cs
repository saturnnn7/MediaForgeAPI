using MediaForge.Library.Application.Abstractions;

namespace MediaForge.Library.Application.Commands.ResolveReport;

public sealed class ResolveReportCommandHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<ResolveReportCommand, Result>
{
    public async Task<Result> Handle(ResolveReportCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
        {
            return Result.Failure(Error.Unauthorized("Only an admin can resolve reports."));
        }

        var report = await reviewRepository.GetReportByIdAsync(request.ReportId, cancellationToken);
        if (report is null)
        {
            return Result.Failure(Error.NotFound("CommentReport", request.ReportId));
        }

        var result = report.Resolve();
        if (result.IsFailure)
        {
            return result;
        }

        reviewRepository.UpdateReport(report);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
