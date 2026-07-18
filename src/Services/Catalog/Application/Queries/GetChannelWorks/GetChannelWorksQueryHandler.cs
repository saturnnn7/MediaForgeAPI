using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetChannelWorks;

public sealed class GetChannelWorksQueryHandler(IWorkRepository workRepository)
    : IRequestHandler<GetChannelWorksQuery, Result<IReadOnlyList<WorkSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<WorkSummaryDto>>> Handle(GetChannelWorksQuery request, CancellationToken cancellationToken)
    {
        var works = await workRepository.GetByChannelIdAsync(request.ChannelId, request.Page, request.PageSize, cancellationToken);
        return Result.Success<IReadOnlyList<WorkSummaryDto>>(works.Select(w => w.ToSummaryDto()).ToList());
    }
}
