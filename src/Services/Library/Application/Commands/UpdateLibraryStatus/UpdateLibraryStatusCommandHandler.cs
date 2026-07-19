using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.UpdateLibraryStatus;

public sealed class UpdateLibraryStatusCommandHandler(
    ILibraryEntryRepository libraryEntryRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<UpdateLibraryStatusCommand, Result<LibraryEntryDto>>
{
    public async Task<Result<LibraryEntryDto>> Handle(UpdateLibraryStatusCommand request, CancellationToken cancellationToken)
    {
        var entry = await libraryEntryRepository.GetByUserAndWorkAsync(currentUserService.UserId, request.WorkId, cancellationToken);
        if (entry is null)
            return Result.Failure<LibraryEntryDto>(Error.NotFound("LibraryEntry", request.WorkId));

        var changeResult = entry.ChangeStatus(request.NewStatus);
        if (changeResult.IsFailure)
            return Result.Failure<LibraryEntryDto>(changeResult.Error);

        libraryEntryRepository.Update(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.ToDto());
    }
}
