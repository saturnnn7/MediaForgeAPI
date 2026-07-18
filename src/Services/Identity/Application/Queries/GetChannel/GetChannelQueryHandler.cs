using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetChannel;

public sealed class GetChannelQueryHandler(
    IChannelRepository channelRepository) : IRequestHandler<GetChannelQuery, Result<ChannelDto>>
{
    public async Task<Result<ChannelDto>> Handle(GetChannelQuery request, CancellationToken cancellationToken)
    {
        var channel = await channelRepository.GetByIdAsync(request.ChannelId, cancellationToken);
        if (channel is null)
        {
            return Result.Failure<ChannelDto>(Error.NotFound("Channel", request.ChannelId));
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
