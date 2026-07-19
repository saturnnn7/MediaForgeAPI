using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetProgress;

public sealed class GetProgressQueryHandler(
    IListeningProgressRepository progressRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetProgressQuery, Result<IReadOnlyList<ListeningProgressDto>>>
{
    public async Task<Result<IReadOnlyList<ListeningProgressDto>>> Handle(GetProgressQuery request, CancellationToken cancellationToken)
    {
        var progresses = await progressRepository.GetByUserAndEditionAsync(currentUserService.UserId, request.EditionId, cancellationToken);
        return Result.Success<IReadOnlyList<ListeningProgressDto>>(progresses.Select(p => p.ToDto()).ToList());
    }
}
