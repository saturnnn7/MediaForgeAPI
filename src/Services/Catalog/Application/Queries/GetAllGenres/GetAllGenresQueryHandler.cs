using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetAllGenres;

public sealed class GetAllGenresQueryHandler(IGenreRepository genreRepository)
    : IRequestHandler<GetAllGenresQuery, Result<IReadOnlyList<GenreDto>>>
{
    public async Task<Result<IReadOnlyList<GenreDto>>> Handle(GetAllGenresQuery request, CancellationToken cancellationToken)
    {
        var genres = await genreRepository.GetAllAsync(cancellationToken);
        return Result.Success<IReadOnlyList<GenreDto>>(genres.Select(g => g.ToDto()).ToList());
    }
}
