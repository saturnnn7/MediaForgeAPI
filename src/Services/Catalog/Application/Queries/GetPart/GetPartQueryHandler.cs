using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetPart;

public sealed class GetPartQueryHandler(IPartRepository partRepository)
    : IRequestHandler<GetPartQuery, Result<PartDetailDto>>
{
    public async Task<Result<PartDetailDto>> Handle(GetPartQuery request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdWithDetailsAsync(request.PartId, cancellationToken);
        return part is null
            ? Result.Failure<PartDetailDto>(Error.NotFound("Part", request.PartId))
            : Result.Success(part.ToDetailDto());
    }
}
