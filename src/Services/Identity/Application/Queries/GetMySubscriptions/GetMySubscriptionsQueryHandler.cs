using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetMySubscriptions;

public sealed class GetMySubscriptionsQueryHandler(
    ICurrentUserService currentUser,
    IChannelRepository channelRepository) : IRequestHandler<GetMySubscriptionsQuery, Result<IReadOnlyList<ChannelDto>>>
{
    public async Task<Result<IReadOnlyList<ChannelDto>>> Handle(GetMySubscriptionsQuery request, CancellationToken cancellationToken)
    {
        var channels = await channelRepository.GetSubscribedChannelsAsync(currentUser.UserId, cancellationToken);

        IReadOnlyList<ChannelDto> dtos = channels
            .Select(c => new ChannelDto(
                c.Id,
                c.Name,
                c.Description,
                c.AvatarUrl,
                c.SubscriberCount,
                c.CreatedAt,
                true))
            .ToList();

        return Result.Success(dtos);
    }
}
