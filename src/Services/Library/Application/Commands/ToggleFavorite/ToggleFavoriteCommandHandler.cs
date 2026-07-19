using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.ToggleFavorite;

public sealed class ToggleFavoriteCommandHandler(
    ILibraryEntryRepository libraryEntryRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<ToggleFavoriteCommand, Result<LibraryEntryDto>>
{
    public async Task<Result<LibraryEntryDto>> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        var entry = await libraryEntryRepository.GetByUserAndWorkAsync(currentUserService.UserId, request.WorkId, cancellationToken);

        if (entry is null)
        {
            var entryResult = LibraryEntry.Create(currentUserService.UserId, request.WorkId, LibraryStatus.Planned);
            if (entryResult.IsFailure)
                return Result.Failure<LibraryEntryDto>(entryResult.Error);

            entry = entryResult.Value;
            entry.ToggleFavorite();

            await libraryEntryRepository.AddAsync(entry, cancellationToken);
        }
        else
        {
            entry.ToggleFavorite();
            libraryEntryRepository.Update(entry);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(entry.ToDto());
    }
}
