using MassTransit;
using MediaForge.Identity.Application.Abstractions;
using MediaForge.Shared.Contracts.Events.Identity;

namespace MediaForge.Identity.Application.Commands.UnsubscribeFromChannel;

public sealed class UnsubscribeFromChannelCommandHandler(
    ICurrentUserService currentUser,
    IChannelRepository channelRepository,
    IIdentityUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<UnsubscribeFromChannelCommand, Result>
{
    public async Task<Result> Handle(UnsubscribeFromChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await channelRepository.GetByIdAsync(request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return Result.Failure(Error.NotFound("Channel", request.ChannelId));
        }

        var isSubscribed = await channelRepository.IsSubscribedAsync(channel.Id, currentUser.UserId, cancellationToken);
        if (!isSubscribed)
        {
            return Result.Failure(Error.Validation("ChannelId", "Not subscribed to this channel."));
        }

        channel.DecrementSubscribers();

        await channelRepository.RemoveSubscriptionAsync(channel.Id, currentUser.UserId, cancellationToken);
        channelRepository.Update(channel);

        await publishEndpoint.Publish(
            new UserUnsubscribedFromChannelEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), channel.Id, currentUser.UserId),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
