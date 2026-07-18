using MassTransit;
using MediaForge.Shared.Contracts.Events.Catalog;

namespace MediaForge.Catalog.Application.Commands.UnpublishWork;

public sealed class UnpublishWorkCommandHandler(
    IWorkRepository workRepository,
    ICatalogUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<UnpublishWorkCommand, Result>
{
    public async Task<Result> Handle(UnpublishWorkCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        work.Unpublish();

        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new WorkUnpublishedEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), work.Id),
            cancellationToken);

        return Result.Success();
    }
}
