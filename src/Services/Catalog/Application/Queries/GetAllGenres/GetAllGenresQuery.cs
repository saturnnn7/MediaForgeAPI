using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetAllGenres;

public sealed record GetAllGenresQuery : IRequest<Result<IReadOnlyList<GenreDto>>>;
