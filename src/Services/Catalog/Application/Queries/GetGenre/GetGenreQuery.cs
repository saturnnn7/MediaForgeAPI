using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetGenre;

public sealed record GetGenreQuery(Guid GenreId) : IRequest<Result<GenreDto>>;
