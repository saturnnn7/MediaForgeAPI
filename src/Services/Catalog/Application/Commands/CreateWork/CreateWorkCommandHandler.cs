using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateWork;

public sealed class CreateWorkCommandHandler(
    IWorkRepository workRepository,
    ISeriesRepository seriesRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<CreateWorkCommand, Result<WorkSummaryDto>>
{
    public async Task<Result<WorkSummaryDto>> Handle(CreateWorkCommand request, CancellationToken cancellationToken)
    {
        var workResult = Work.Create(request.ChannelId, currentUserService.UserId, request.WorkType, request.Title);
        if (workResult.IsFailure)
            return Result.Failure<WorkSummaryDto>(workResult.Error);

        var work = workResult.Value;

        if (request.Description is not null || request.CoverUrl is not null || request.Language is not null)
        {
            var updateResult = work.UpdateDetails(request.Title, request.Description, request.CoverUrl, request.Language);
            if (updateResult.IsFailure)
                return Result.Failure<WorkSummaryDto>(updateResult.Error);
        }

        if (request.SeriesId is { } seriesId)
        {
            var series = await seriesRepository.GetByIdAsync(seriesId, cancellationToken);
            if (series is null)
                return Result.Failure<WorkSummaryDto>(Error.NotFound("Series", seriesId));

            work.AssignToSeries(seriesId);
        }

        await workRepository.AddAsync(work, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(work.ToSummaryDto());
    }
}
