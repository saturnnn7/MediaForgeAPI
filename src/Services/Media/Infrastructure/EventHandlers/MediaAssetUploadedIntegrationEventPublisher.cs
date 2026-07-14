using MediaForge.Media.Domain.Events;
using MediatR;

namespace MediaForge.Media.Infrastructure.EventHandlers;

public sealed class MediaAssetUploadedIntegrationEventPublisher
    : INotificationHandler<MediaAssetUploadedDomainEvent>
{
    // No-op: MediaUploadedEvent is now published directly from ConfirmUploadCommandHandler
    // via IPublishEndpoint so it participates in the EF Core outbox transaction.
    public Task Handle(MediaAssetUploadedDomainEvent notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
