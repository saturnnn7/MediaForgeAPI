namespace MediaForge.Library.Application.Commands.ReportContent;

public sealed class ReportContentCommandHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<ReportContentCommand, Result>
{
    public async Task<Result> Handle(ReportContentCommand request, CancellationToken cancellationToken)
    {
        var report = CommentReport.Create(request.TargetId, request.TargetType, currentUserService.UserId, request.Reason);

        await reviewRepository.AddReportAsync(report, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
