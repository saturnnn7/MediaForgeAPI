namespace MediaForge.Catalog.Application.Commands.PublishWork;

public sealed class PublishWorkCommandHandler(
    IWorkRepository workRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<PublishWorkCommand, Result>
{
    public async Task<Result> Handle(PublishWorkCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        var publishResult = work.Publish();
        if (publishResult.IsFailure)
            return publishResult;

        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
