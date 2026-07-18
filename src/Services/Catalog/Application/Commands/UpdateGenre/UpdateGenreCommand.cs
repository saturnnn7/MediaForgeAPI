using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateGenre;

public sealed record UpdateGenreCommand(Guid GenreId, string Name, string? Description) : IRequest<Result<GenreDto>>;
