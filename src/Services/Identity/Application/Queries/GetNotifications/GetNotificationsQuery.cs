using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetNotifications;

public sealed record GetNotificationsQuery(int Page = 1, int PageSize = 20) : IRequest<Result<IReadOnlyList<NotificationDto>>>;
