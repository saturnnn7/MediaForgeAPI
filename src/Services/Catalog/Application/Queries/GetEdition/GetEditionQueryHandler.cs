using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetEdition;

public sealed class GetEditionQueryHandler(IEditionRepository editionRepository)
    : IRequestHandler<GetEditionQuery, Result<EditionDto>>
{
    public async Task<Result<EditionDto>> Handle(GetEditionQuery request, CancellationToken cancellationToken)
    {
        var edition = await editionRepository.GetByIdAsync(request.EditionId, cancellationToken);
        return edition is null
            ? Result.Failure<EditionDto>(Error.NotFound("Edition", request.EditionId))
            : Result.Success(edition.ToDto());
    }
}
