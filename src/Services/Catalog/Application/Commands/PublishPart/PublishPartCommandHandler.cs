namespace MediaForge.Catalog.Application.Commands.PublishPart;

public sealed class PublishPartCommandHandler(
    IPartRepository partRepository,
    IWorkRepository workRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<PublishPartCommand, Result>
{
    public async Task<Result> Handle(PublishPartCommand request, CancellationToken cancellationToken)
    {
        var part = await partRepository.GetByIdAsync(request.PartId, cancellationToken);
        if (part is null)
            return Result.Failure(Error.NotFound("Part", request.PartId));

        var work = await workRepository.GetByIdAsync(part.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", part.WorkId));

        if (work.CreatorId != currentUserService.UserId)
            return Result.Failure(Error.Unauthorized("Only the channel owner can publish this part."));

        part.Publish();

        partRepository.Update(part);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
