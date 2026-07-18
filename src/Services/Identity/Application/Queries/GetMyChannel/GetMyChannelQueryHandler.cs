using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetMyChannel;

public sealed class GetMyChannelQueryHandler(
    ICurrentUserService currentUser,
    IChannelRepository channelRepository) : IRequestHandler<GetMyChannelQuery, Result<ChannelDto>>
{
    public async Task<Result<ChannelDto>> Handle(GetMyChannelQuery request, CancellationToken cancellationToken)
    {
        var channel = await channelRepository.GetByOwnerIdAsync(currentUser.UserId, cancellationToken);
        if (channel is null)
        {
            return Result.Failure<ChannelDto>(Error.NotFound("Channel", currentUser.UserId));
        }

        return Result.Success(new ChannelDto(
            channel.Id,
            channel.Name,
            channel.Description,
            channel.AvatarUrl,
            channel.SubscriberCount,
            channel.CreatedAt,
            false));
    }
}
