namespace MediaForge.Catalog.Application.Commands.RemoveContributor;

public sealed class RemoveContributorCommandHandler(
    IWorkRepository workRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<RemoveContributorCommand, Result>
{
    public async Task<Result> Handle(RemoveContributorCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        var removeResult = work.RemoveContributor(request.PersonId, request.Role);
        if (removeResult.IsFailure)
            return removeResult;

        await workRepository.RemoveContributorAsync(request.WorkId, request.PersonId, request.Role, cancellationToken);
        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
