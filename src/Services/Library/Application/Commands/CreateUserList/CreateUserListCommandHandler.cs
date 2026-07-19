using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.CreateUserList;

public sealed class CreateUserListCommandHandler(
    IUserListRepository userListRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<CreateUserListCommand, Result<UserListDto>>
{
    public async Task<Result<UserListDto>> Handle(CreateUserListCommand request, CancellationToken cancellationToken)
    {
        var existing = await userListRepository.GetBySlugAsync(currentUserService.UserId, request.Slug, cancellationToken);
        if (existing is not null)
            return Result.Failure<UserListDto>(Error.Conflict("UserList", "A list with this slug already exists."));

        var listResult = UserList.Create(currentUserService.UserId, request.Name, request.Slug);
        if (listResult.IsFailure)
            return Result.Failure<UserListDto>(listResult.Error);

        var list = listResult.Value;

        var updateResult = list.UpdateDetails(request.Name, request.Description, request.AvatarUrl);
        if (updateResult.IsFailure)
            return Result.Failure<UserListDto>(updateResult.Error);

        list.SetPrivacy(request.Privacy);

        await userListRepository.AddAsync(list, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(list.ToDto());
    }
}
