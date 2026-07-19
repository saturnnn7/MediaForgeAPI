using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.RateWork;

public sealed record RateWorkCommand(Guid WorkId, int? Rating) : IRequest<Result<LibraryEntryDto>>;
