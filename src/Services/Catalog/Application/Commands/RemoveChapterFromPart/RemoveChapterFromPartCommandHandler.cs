namespace MediaForge.Catalog.Application.Commands.RemoveChapterFromPart;

public sealed class RemoveChapterFromPartCommandHandler(
    IPartRepository partRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<RemoveChapterFromPartCommand, Result>
{
    public async Task<Result> Handle(RemoveChapterFromPartCommand request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdWithDetailsAsync(request.PartId, cancellationToken);
        if (part is null)
            return Result.Failure(Error.NotFound("Part", request.PartId));

        var removeResult = part.RemoveChapter(request.ChapterId);
        if (removeResult.IsFailure)
            return removeResult;

        await partRepository.RemoveChapterAsync(request.ChapterId, cancellationToken);
        partRepository.Update(part);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
