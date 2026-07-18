using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetSeriesWorks;

public sealed class GetSeriesWorksQueryHandler(IWorkRepository workRepository)
    : IRequestHandler<GetSeriesWorksQuery, Result<IReadOnlyList<WorkSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<WorkSummaryDto>>> Handle(GetSeriesWorksQuery request, CancellationToken cancellationToken)
    {
        var works = await workRepository.GetBySeriesIdAsync(request.SeriesId, cancellationToken);
        return Result.Success<IReadOnlyList<WorkSummaryDto>>(works.Select(w => w.ToSummaryDto()).ToList());
    }
}
