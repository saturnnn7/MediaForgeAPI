using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler(
    ICurrentUserService currentUser,
    INotificationRepository notificationRepository) : IRequestHandler<GetNotificationsQuery, Result<IReadOnlyList<NotificationDto>>>
{
    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await notificationRepository.GetByUserIdAsync(currentUser.UserId, request.Page, request.PageSize, cancellationToken);

        IReadOnlyList<NotificationDto> dtos = notifications
            .Select(n => new NotificationDto(n.Id, n.UserId, n.Type.ToString(), n.Payload, n.IsRead, n.CreatedAt))
            .ToList();

        return Result.Success(dtos);
    }
}
