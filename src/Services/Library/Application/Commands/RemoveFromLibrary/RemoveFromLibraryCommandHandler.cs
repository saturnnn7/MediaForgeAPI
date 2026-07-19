namespace MediaForge.Library.Application.Commands.RemoveFromLibrary;

public sealed class RemoveFromLibraryCommandHandler(
    ILibraryEntryRepository libraryEntryRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<RemoveFromLibraryCommand, Result>
{
    public async Task<Result> Handle(RemoveFromLibraryCommand request, CancellationToken cancellationToken)
    {
        var entry = await libraryEntryRepository.GetByUserAndWorkAsync(currentUserService.UserId, request.WorkId, cancellationToken);
        if (entry is null)
            return Result.Failure(Error.NotFound("LibraryEntry", request.WorkId));

        await libraryEntryRepository.DeleteAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
