using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.MarkAllNotificationsRead;

public sealed class MarkAllNotificationsReadCommandHandler(
    ICurrentUserService currentUser,
    INotificationRepository notificationRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        await notificationRepository.MarkAllReadAsync(currentUser.UserId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
