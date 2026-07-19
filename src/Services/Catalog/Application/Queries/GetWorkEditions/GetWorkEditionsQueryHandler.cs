using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetWorkEditions;

public sealed class GetWorkEditionsQueryHandler(IEditionRepository editionRepository)
    : IRequestHandler<GetWorkEditionsQuery, Result<IReadOnlyList<EditionWithPartsDto>>>
{
    public async Task<Result<IReadOnlyList<EditionWithPartsDto>>> Handle(GetWorkEditionsQuery request, CancellationToken cancellationToken)
    {
        var editions = await editionRepository.GetByWorkIdAsync(request.WorkId, cancellationToken);

        return Result.Success<IReadOnlyList<EditionWithPartsDto>>(editions
            .Select(e => e.ToWithPartsDto())
            .ToList());
    }
}
