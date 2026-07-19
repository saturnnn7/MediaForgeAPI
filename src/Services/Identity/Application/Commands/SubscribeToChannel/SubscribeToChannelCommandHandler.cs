using MassTransit;
using MediaForge.Identity.Application.Abstractions;
using MediaForge.Shared.Contracts.Events.Identity;

namespace MediaForge.Identity.Application.Commands.SubscribeToChannel;

public sealed class SubscribeToChannelCommandHandler(
    ICurrentUserService currentUser,
    IChannelRepository channelRepository,
    IIdentityUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint) : IRequestHandler<SubscribeToChannelCommand, Result>
{
    public async Task<Result> Handle(SubscribeToChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await channelRepository.GetByIdAsync(request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return Result.Failure(Error.NotFound("Channel", request.ChannelId));
        }

        if (channel.OwnerId == currentUser.UserId)
        {
            return Result.Failure(Error.Validation("ChannelId", "You cannot subscribe to your own channel."));
        }

        var alreadySubscribed = await channelRepository.IsSubscribedAsync(channel.Id, currentUser.UserId, cancellationToken);
        if (alreadySubscribed)
        {
            return Result.Failure(Error.Conflict("Subscription", "Already subscribed to this channel."));
        }

        var subscription = ChannelSubscription.Create(channel.Id, currentUser.UserId);
        channel.IncrementSubscribers();

        await channelRepository.AddSubscriptionAsync(subscription, cancellationToken);
        channelRepository.Update(channel);

        await publishEndpoint.Publish(
            new UserSubscribedToChannelEvent(Guid.NewGuid(), DateTime.UtcNow, Guid.NewGuid(), channel.Id, currentUser.UserId),
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
