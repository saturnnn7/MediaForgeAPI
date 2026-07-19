using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.AddToLibrary;

public sealed record AddToLibraryCommand(
    Guid WorkId,
    LibraryStatus Status,
    ListPrivacy Privacy = ListPrivacy.Everyone) : IRequest<Result<LibraryEntryDto>>;
