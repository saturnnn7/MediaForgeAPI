namespace MediaForge.Catalog.Application.Commands.UnpublishWork;

public sealed class UnpublishWorkCommandHandler(
    IWorkRepository workRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UnpublishWorkCommand, Result>
{
    public async Task<Result> Handle(UnpublishWorkCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        work.Unpublish();

        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
