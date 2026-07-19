using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.UpdateUserList;

public sealed class UpdateUserListCommandHandler(
    IUserListRepository userListRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<UpdateUserListCommand, Result<UserListDto>>
{
    public async Task<Result<UserListDto>> Handle(UpdateUserListCommand request, CancellationToken cancellationToken)
    {
        var list = await userListRepository.GetByIdAsync(request.ListId, cancellationToken);
        if (list is null)
            return Result.Failure<UserListDto>(Error.NotFound("UserList", request.ListId));

        if (list.UserId != currentUserService.UserId)
            return Result.Failure<UserListDto>(Error.Unauthorized("You do not own this list."));

        var updateResult = list.UpdateDetails(request.Name, request.Description, request.AvatarUrl);
        if (updateResult.IsFailure)
            return Result.Failure<UserListDto>(updateResult.Error);

        list.SetPrivacy(request.Privacy);

        userListRepository.Update(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(list.ToDto());
    }
}
