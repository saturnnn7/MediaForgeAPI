namespace MediaForge.Catalog.Application.Commands.RemoveGenreFromWork;

public sealed class RemoveGenreFromWorkCommandHandler(
    IWorkRepository workRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<RemoveGenreFromWorkCommand, Result>
{
    public async Task<Result> Handle(RemoveGenreFromWorkCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        var removeResult = work.RemoveGenre(request.GenreId);
        if (removeResult.IsFailure)
            return removeResult;

        await workRepository.RemoveGenreAsync(request.WorkId, request.GenreId, cancellationToken);
        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
