using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetMyLists;

public sealed class GetMyListsQueryHandler(
    IUserListRepository userListRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyListsQuery, Result<IReadOnlyList<UserListDto>>>
{
    public async Task<Result<IReadOnlyList<UserListDto>>> Handle(GetMyListsQuery request, CancellationToken cancellationToken)
    {
        var lists = await userListRepository.GetByUserIdAsync(currentUserService.UserId, cancellationToken);
        return Result.Success<IReadOnlyList<UserListDto>>(lists.Select(l => l.ToDto()).ToList());
    }
}
