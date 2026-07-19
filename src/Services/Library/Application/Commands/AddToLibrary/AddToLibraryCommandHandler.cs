using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.AddToLibrary;

public sealed class AddToLibraryCommandHandler(
    ILibraryEntryRepository libraryEntryRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<AddToLibraryCommand, Result<LibraryEntryDto>>
{
    public async Task<Result<LibraryEntryDto>> Handle(AddToLibraryCommand request, CancellationToken cancellationToken)
    {
        var existing = await libraryEntryRepository.GetByUserAndWorkAsync(currentUserService.UserId, request.WorkId, cancellationToken);
        if (existing is not null)
            return Result.Failure<LibraryEntryDto>(Error.Conflict("LibraryEntry", "This work is already in your library."));

        var entryResult = LibraryEntry.Create(currentUserService.UserId, request.WorkId, request.Status);
        if (entryResult.IsFailure)
            return Result.Failure<LibraryEntryDto>(entryResult.Error);

        var entry = entryResult.Value;
        entry.SetPrivacy(request.Privacy);

        await libraryEntryRepository.AddAsync(entry, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.ToDto());
    }
}
