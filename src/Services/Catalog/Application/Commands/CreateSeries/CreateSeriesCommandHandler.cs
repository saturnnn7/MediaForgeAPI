using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateSeries;

public sealed class CreateSeriesCommandHandler(
    ISeriesRepository seriesRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<CreateSeriesCommand, Result<SeriesDto>>
{
    public async Task<Result<SeriesDto>> Handle(CreateSeriesCommand request, CancellationToken cancellationToken)
    {
        var seriesResult = Series.Create(request.ChannelId, request.Title);
        if (seriesResult.IsFailure)
            return Result.Failure<SeriesDto>(seriesResult.Error);

        var series = seriesResult.Value;

        if (request.Description is not null || request.CoverUrl is not null)
        {
            var updateResult = series.UpdateDetails(request.Title, request.Description, request.CoverUrl);
            if (updateResult.IsFailure)
                return Result.Failure<SeriesDto>(updateResult.Error);
        }

        await seriesRepository.AddAsync(series, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(series.ToDto());
    }
}
