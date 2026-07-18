using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetGenre;

public sealed class GetGenreQueryHandler(IGenreRepository genreRepository)
    : IRequestHandler<GetGenreQuery, Result<GenreDto>>
{
    public async Task<Result<GenreDto>> Handle(GetGenreQuery request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetByIdAsync(request.GenreId, cancellationToken);
        return genre is null
            ? Result.Failure<GenreDto>(Error.NotFound("Genre", request.GenreId))
            : Result.Success(genre.ToDto());
    }
}
