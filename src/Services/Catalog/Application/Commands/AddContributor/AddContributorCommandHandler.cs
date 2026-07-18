namespace MediaForge.Catalog.Application.Commands.AddContributor;

public sealed class AddContributorCommandHandler(
    IWorkRepository workRepository,
    IPersonRepository personRepository,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<AddContributorCommand, Result>
{
    public async Task<Result> Handle(AddContributorCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdWithDetailsAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        var person = await personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result.Failure(Error.NotFound("Person", request.PersonId));

        var addResult = work.AddContributor(request.PersonId, request.Role);
        if (addResult.IsFailure)
            return addResult;

        var contributor = work.Contributors[^1];
        await workRepository.AddContributorAsync(contributor, cancellationToken);
        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
