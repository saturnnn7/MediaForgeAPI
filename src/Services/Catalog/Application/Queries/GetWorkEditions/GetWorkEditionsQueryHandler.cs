using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkEditions;

public sealed class GetWorkEditionsQueryHandler(IEditionRepository editionRepository)
    : IRequestHandler<GetWorkEditionsQuery, Result<IReadOnlyList<EditionWithPartsDto>>>
{
    public async Task<Result<IReadOnlyList<EditionWithPartsDto>>> Handle(GetWorkEditionsQuery request, CancellationToken cancellationToken)
    {
        var editions = await editionRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);

        var result = new List<EditionWithPartsDto>(editions.Count);
        foreach (var edition in editions)
        {
            var editionWithParts = await editionRepository.GetByIdWithPartsAsync(edition.Id, cancellationToken);
            if (editionWithParts is not null)
                result.Add(editionWithParts.ToWithPartsDto());
        }

        return Result.Success<IReadOnlyList<EditionWithPartsDto>>(result);
    }
}
