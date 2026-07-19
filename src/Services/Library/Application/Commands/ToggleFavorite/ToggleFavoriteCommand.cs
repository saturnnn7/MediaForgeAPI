using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.ToggleFavorite;

public sealed record ToggleFavoriteCommand(Guid WorkId) : IRequest<Result<LibraryEntryDto>>;
