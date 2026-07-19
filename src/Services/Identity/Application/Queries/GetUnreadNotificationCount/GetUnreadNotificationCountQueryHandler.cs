using MediaForge.Identity.Application.Abstractions;

namespace MediaForge.Identity.Application.Queries.GetUnreadNotificationCount;

public sealed class GetUnreadNotificationCountQueryHandler(
    ICurrentUserService currentUser,
    INotificationRepository notificationRepository) : IRequestHandler<GetUnreadNotificationCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        var count = await notificationRepository.GetUnreadCountAsync(currentUser.UserId, cancellationToken);
        return Result.Success(count);
    }
}
