using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetChannelSeries;

public sealed class GetChannelSeriesQueryHandler(ISeriesRepository seriesRepository)
    : IRequestHandler<GetChannelSeriesQuery, Result<IReadOnlyList<SeriesDto>>>
{
    public async Task<Result<IReadOnlyList<SeriesDto>>> Handle(GetChannelSeriesQuery request, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByChannelIdAsync(request.ChannelId, request.Page, request.PageSize, cancellationToken);
        return Result.Success<IReadOnlyList<SeriesDto>>(series.Select(s => s.ToDto()).ToList());
    }
}
