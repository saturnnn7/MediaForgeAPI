using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetMyFollows;

public sealed class GetMyFollowsQueryHandler(
    ICurrentUserService currentUser,
    IAuthorFollowRepository authorFollowRepository) : IRequestHandler<GetMyFollowsQuery, Result<IReadOnlyList<AuthorFollowDto>>>
{
    public async Task<Result<IReadOnlyList<AuthorFollowDto>>> Handle(GetMyFollowsQuery request, CancellationToken cancellationToken)
    {
        var follows = await authorFollowRepository.GetByUserIdAsync(currentUser.UserId, cancellationToken);

        IReadOnlyList<AuthorFollowDto> dtos = follows
            .Select(f => new AuthorFollowDto(f.Id, f.UserId, f.PersonId, f.FollowedAt))
            .ToList();

        return Result.Success(dtos);
    }
}
