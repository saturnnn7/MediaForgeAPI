using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetList;

public sealed class GetListQueryHandler(
    IUserListRepository userListRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetListQuery, Result<UserListDetailDto>>
{
    public async Task<Result<UserListDetailDto>> Handle(GetListQuery request, CancellationToken cancellationToken)
    {
        var list = await userListRepository.GetByIdWithItemsAsync(request.ListId, cancellationToken);
        if (list is null)
            return Result.Failure<UserListDetailDto>(Error.NotFound("UserList", request.ListId));

        var isOwner = list.UserId == currentUserService.UserId;

        if (list.Privacy == ListPrivacy.Nobody && !isOwner)
            return Result.Failure<UserListDetailDto>(Error.Unauthorized("This list is private."));

        if (list.Privacy == ListPrivacy.Authorized && !currentUserService.IsAuthenticated)
            return Result.Failure<UserListDetailDto>(Error.Unauthorized("You must be signed in to view this list."));

        return Result.Success(list.ToDetailDto());
    }
}
