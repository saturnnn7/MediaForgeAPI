using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Commands.MarkNotificationRead;

public sealed class MarkNotificationReadCommandHandler(
    ICurrentUserService currentUser,
    INotificationRepository notificationRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<MarkNotificationReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification is null)
        {
            return Result.Failure(Error.NotFound("Notification", request.NotificationId));
        }

        if (notification.UserId != currentUser.UserId)
        {
            return Result.Failure(Error.Unauthorized("You cannot modify this notification."));
        }

        notification.MarkAsRead();
        notificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
