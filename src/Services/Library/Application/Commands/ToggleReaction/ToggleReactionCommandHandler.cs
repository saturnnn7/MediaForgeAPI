namespace MediaForge.Library.Application.Commands.ToggleReaction;

public sealed class ToggleReactionCommandHandler(
    IReviewRepository reviewRepository,
    ICurrentUserService currentUserService,
    ILibraryUnitOfWork unitOfWork) : IRequestHandler<ToggleReactionCommand, Result>
{
    public async Task<Result> Handle(ToggleReactionCommand request, CancellationToken cancellationToken)
    {
        var existing = await reviewRepository.GetReactionAsync(request.TargetId, currentUserService.UserId, request.Emoji, cancellationToken);

        if (existing is not null)
        {
            await reviewRepository.RemoveReactionAsync(request.TargetId, currentUserService.UserId, request.Emoji, cancellationToken);
        }
        else
        {
            var reaction = ReviewReaction.Create(request.TargetId, request.TargetType, currentUserService.UserId, request.Emoji);
            await reviewRepository.AddReactionAsync(reaction, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
