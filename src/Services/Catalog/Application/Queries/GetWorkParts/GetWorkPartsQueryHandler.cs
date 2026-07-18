using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkParts;

public sealed class GetWorkPartsQueryHandler(IPartRepository partRepository)
    : IRequestHandler<GetWorkPartsQuery, Result<IReadOnlyList<PartSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<PartSummaryDto>>> Handle(GetWorkPartsQuery request, CancellationToken cancellationToken)
    {
        var parts = await partRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
        return Result.Success<IReadOnlyList<PartSummaryDto>>(parts.Select(p => p.ToSummaryDto()).ToList());
    }
}
