using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetSeries;

public sealed class GetSeriesQueryHandler(ISeriesRepository seriesRepository)
    : IRequestHandler<GetSeriesQuery, Result<SeriesDto>>
{
    public async Task<Result<SeriesDto>> Handle(GetSeriesQuery request, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(request.SeriesId, cancellationToken);
        return series is null
            ? Result.Failure<SeriesDto>(Error.NotFound("Series", request.SeriesId))
            : Result.Success(series.ToDto());
    }
}
