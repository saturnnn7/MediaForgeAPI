namespace MediaForge.Library.Application.Commands.AddWorkToList;

public sealed class AddWorkToListCommandHandler(
    IUserListRepository userListRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<AddWorkToListCommand, Result>
{
    public async Task<Result> Handle(AddWorkToListCommand request, CancellationToken cancellationToken)
    {
        var list = await userListRepository.GetByIdWithItemsAsync(request.ListId, cancellationToken);
        if (list is null)
            return Result.Failure(Error.NotFound("UserList", request.ListId));

        if (list.UserId != currentUserService.UserId)
            return Result.Failure(Error.Unauthorized("You do not own this list."));

        var addResult = list.AddItem(request.WorkId, request.DisplayOrder);
        if (addResult.IsFailure)
            return Result.Failure(addResult.Error);

        await userListRepository.AddItemAsync(addResult.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
