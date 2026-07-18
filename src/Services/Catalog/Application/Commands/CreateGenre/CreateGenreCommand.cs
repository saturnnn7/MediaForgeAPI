using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateGenre;

public sealed record CreateGenreCommand(string Name, string Slug, string? Description) : IRequest<Result<GenreDto>>;
