namespace MediaForge.Catalog.Application.Commands.SetWorkPrivacy;

public sealed class SetWorkPrivacyCommandHandler(
    IWorkRepository workRepository,
    ICurrentUserService currentUserService,
    ICatalogUnitOfWork unitOfWork) : IRequestHandler<SetWorkPrivacyCommand, Result>
{
    public async Task<Result> Handle(SetWorkPrivacyCommand request, CancellationToken cancellationToken)
    {
        var work = await workRepository.GetByIdAsync(request.WorkId, cancellationToken);
        if (work is null)
            return Result.Failure(Error.NotFound("Work", request.WorkId));

        if (work.ChannelId != currentUserService.UserId)
            return Result.Failure(Error.Unauthorized("Only the channel owner can change this work's privacy."));

        if (request.IsPrivate)
            work.MakePrivate();
        else
            work.MakePublic();

        workRepository.Update(work);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
