namespace MediaForge.Catalog.Application.Commands.UnpublishPart;

public sealed class UnpublishPartCommandHandler(
    IPartRepository partRepository,
    IWorkRepository workRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<UnpublishPartCommand, Result>
{
    public async Task<Result> Handle(UnpublishPartCommand request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdAsync(request.PartId, cancellationToken);
        if (part is null)
            return Result.Failure(Error.NotFound("Part", request.PartId));

        var work = await workRepository.GetByIdAsync(part.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", part.WorkId));

        if (work.CreatorId != currentUserService.UserId)
            return Result.Failure(Error.Unauthorized("Only the channel owner can unpublish this part."));

        part.Unpublish();

        partRepository.Update(part);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
