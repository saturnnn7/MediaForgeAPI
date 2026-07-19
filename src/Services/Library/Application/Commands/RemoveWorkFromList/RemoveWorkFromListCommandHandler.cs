namespace MediaForge.Library.Application.Commands.RemoveWorkFromList;

public sealed class RemoveWorkFromListCommandHandler(
    IUserListRepository userListRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<RemoveWorkFromListCommand, Result>
{
    public async Task<Result> Handle(RemoveWorkFromListCommand request, CancellationToken cancellationToken)
    {
        var list = await userListRepository.GetByIdWithItemsAsync(request.ListId, cancellationToken);
        if (list is null)
            return Result.Failure(Error.NotFound("UserList", request.ListId));

        if (list.UserId != currentUserService.UserId)
            return Result.Failure(Error.Unauthorized("You do not own this list."));

        var removeResult = list.RemoveItem(request.WorkId);
        if (removeResult.IsFailure)
            return Result.Failure(removeResult.Error);

        await userListRepository.RemoveItemAsync(request.ListId, request.WorkId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
