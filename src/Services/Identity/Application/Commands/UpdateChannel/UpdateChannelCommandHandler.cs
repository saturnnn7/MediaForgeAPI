using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.UpdateChannel;

public sealed class UpdateChannelCommandHandler(
    ICurrentUserService currentUser,
    IChannelRepository channelRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<UpdateChannelCommand, Result<ChannelDto>>
{
    public async Task<Result<ChannelDto>> Handle(UpdateChannelCommand request, CancellationToken cancellationToken)
    {
        var channel = await channelRepository.GetByOwnerIdAsync(currentUser.UserId, cancellationToken);
        if (channel is null)
        {
            return Result.Failure<ChannelDto>(Error.NotFound("Channel", currentUser.UserId));
        }

        var result = channel.UpdateProfile(request.Name, request.Description, request.AvatarUrl);
        if (result.IsFailure)
        {
            return Result.Failure<ChannelDto>(result.Error);
        }

        channelRepository.Update(channel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

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
