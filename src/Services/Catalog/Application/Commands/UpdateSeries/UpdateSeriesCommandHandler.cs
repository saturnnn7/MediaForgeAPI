using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateSeries;

public sealed class UpdateSeriesCommandHandler(
    ISeriesRepository seriesRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UpdateSeriesCommand, Result<SeriesDto>>
{
    public async Task<Result<SeriesDto>> Handle(UpdateSeriesCommand request, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(request.SeriesId, cancellationToken);
        if (series is null)
            return Result.Failure<SeriesDto>(Error.NotFound("Series", request.SeriesId));

        var updateResult = series.UpdateDetails(request.Title, request.Description, request.CoverUrl);
        if (updateResult.IsFailure)
            return Result.Failure<SeriesDto>(updateResult.Error);

        seriesRepository.Update(series);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(series.ToDto());
    }
}
