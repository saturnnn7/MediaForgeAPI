namespace MediaForge.Catalog.Application.Commands.AddGenreToWork;

public sealed class AddGenreToWorkCommandHandler(
    IWorkRepository workRepository,
    IGenreRepository genreRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<AddGenreToWorkCommand, Result>
{
    public async Task<Result> Handle(AddGenreToWorkCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        var genre = await genreRepository.GetByIdAsync(request.GenreId, cancellationToken);
        if (genre is null)
            return Result.Failure(Error.NotFound("Genre", request.GenreId));

        var addResult = work.AddGenre(request.GenreId);
        if (addResult.IsFailure)
            return addResult;

        var workGenre = work.Genres[^1];
        await workRepository.AddGenreAsync(workGenre, cancellationToken);
        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
