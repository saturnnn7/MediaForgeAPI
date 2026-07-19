using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetMyLibrary;

public sealed class GetMyLibraryQueryHandler(
    ILibraryEntryRepository libraryEntryRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetMyLibraryQuery, Result<IReadOnlyList<LibraryEntryDto>>>
{
    public async Task<Result<IReadOnlyList<LibraryEntryDto>>> Handle(GetMyLibraryQuery request, CancellationToken cancellationToken)
    {
        var entries = await libraryEntryRepository.GetByUserIdAsync(currentUserService.UserId, cancellationToken);
        return Result.Success<IReadOnlyList<LibraryEntryDto>>(entries.Select(e => e.ToDto()).ToList());
    }
}
