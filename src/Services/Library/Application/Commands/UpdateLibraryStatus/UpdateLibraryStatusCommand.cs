using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.UpdateLibraryStatus;

public sealed record UpdateLibraryStatusCommand(Guid WorkId, LibraryStatus NewStatus) : IRequest<Result<LibraryEntryDto>>;
