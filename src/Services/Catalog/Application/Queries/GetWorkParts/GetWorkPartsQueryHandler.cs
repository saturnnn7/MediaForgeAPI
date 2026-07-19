using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkParts;

public sealed class GetWorkPartsQueryHandler(
    IPartRepository partRepository,
    IEditionRepository editionRepository)
    : IRequestHandler<GetWorkPartsQuery, Result<IReadOnlyList<PartSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<PartSummaryDto>>> Handle(GetWorkPartsQuery request, CancellationToken cancellationToken)
    {
        var parts = await partRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
        var editions = await editionRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);
        var narratorTeamNameByEditionId = editions.ToDictionary(e => e.Id, e => e.NarratorTeamName);

        return Result.Success<IReadOnlyList<PartSummaryDto>>(parts
            .Select(p => p.ToSummaryDto(narratorTeamNameByEditionId.GetValueOrDefault(p.EditionId)))
            .ToList());
    }
}
