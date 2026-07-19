using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Commands.UpdateProgress;

public sealed class UpdateProgressCommandHandler(
    IListeningProgressRepository progressRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<UpdateProgressCommand, Result<ListeningProgressDto>>
{
    public async Task<Result<ListeningProgressDto>> Handle(UpdateProgressCommand request, CancellationToken cancellationToken)
    {
        var progress = await progressRepository.GetAsync(currentUserService.UserId, request.EditionId, request.PartId, cancellationToken);

        if (progress is null)
        {
            progress = ListeningProgress.Create(currentUserService.UserId, request.EditionId, request.PartId, request.PositionSeconds);
            await progressRepository.AddAsync(progress, cancellationToken);
        }
        else
        {
            progress.UpdatePosition(request.PositionSeconds);
            progressRepository.Update(progress);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(progress.ToDto());
    }
}
