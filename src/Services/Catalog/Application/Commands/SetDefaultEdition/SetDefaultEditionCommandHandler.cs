namespace MediaForge.Catalog.Application.Commands.SetDefaultEdition;

public sealed class SetDefaultEditionCommandHandler(
    IEditionRepository editionRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<SetDefaultEditionCommand, Result>
{
    public async Task<Result> Handle(SetDefaultEditionCommand request, CancellationToken cancellationToken)
    {
        if (currentUserService.Role != "admin")
            return Result.Failure(Error.Unauthorized("Only an admin can change the default edition."));

        var edition = await editionRepository.GetByIdAsync(request.EditionId, cancellationToken);
        if (edition is null)
            return Result.Failure(Error.NotFound("Edition", request.EditionId));

        var workEditions = await editionRepository.GetByWorkIdAsync(edition.WorkId, cancellationToken);

        foreach (var workEdition in workEditions)
        {
            workEdition.UnsetDefault();
            editionRepository.Update(workEdition);
        }

        edition.SetAsDefault();
        editionRepository.Update(edition);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
