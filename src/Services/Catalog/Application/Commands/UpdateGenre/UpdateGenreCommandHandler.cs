using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Commands.UpdateGenre;

public sealed class UpdateGenreCommandHandler(
    IGenreRepository genreRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UpdateGenreCommand, Result<GenreDto>>
{
    public async Task<Result<GenreDto>> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await genreRepository.GetByIdAsync(request.GenreId, cancellationToken);
        if (genre is null)
            return Result.Failure<GenreDto>(Error.NotFound("Genre", request.GenreId));

        var updateResult = genre.Update(request.Name, request.Description);
        if (updateResult.IsFailure)
            return Result.Failure<GenreDto>(updateResult.Error);

        genreRepository.Update(genre);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(genre.ToDto());
    }
}
