using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.CreateGenre;

public sealed class CreateGenreCommandHandler(
    IGenreRepository genreRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<CreateGenreCommand, Result<GenreDto>>
{
    public async Task<Result<GenreDto>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var existing = await genreRepository.GetBySlugAsync(request.Slug, cancellationToken);
        if (existing is not null)
            return Result.Failure<GenreDto>(Error.Conflict("Genre", "A genre with this slug already exists."));

        var genreResult = Genre.Create(request.Name, request.Slug);
        if (genreResult.IsFailure)
            return Result.Failure<GenreDto>(genreResult.Error);

        var genre = genreResult.Value;

        if (request.Description is not null)
        {
            var updateResult = genre.Update(request.Name, request.Description);
            if (updateResult.IsFailure)
                return Result.Failure<GenreDto>(updateResult.Error);
        }

        await genreRepository.AddAsync(genre, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(genre.ToDto());
    }
}
