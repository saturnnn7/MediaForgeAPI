using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.CreateChannel;

public sealed class CreateChannelCommandHandler(
    ICurrentUserService currentUser,
    IApplicationUserRepository userRepository,
    IChannelRepository channelRepository,
    IIdentityUnitOfWork unitOfWork) : IRequestHandler<CreateChannelCommand, Result<ChannelDto>>
{
    public async Task<Result<ChannelDto>> Handle(CreateChannelCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdWithChannelAsync(currentUser.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<ChannelDto>(Error.NotFound("User", currentUser.UserId));
        }

        var result = user.CreateChannel(request.Name);
        if (result.IsFailure)
        {
            return Result.Failure<ChannelDto>(result.Error);
        }

        var channel = result.Value;

        await channelRepository.AddAsync(channel, cancellationToken);
        userRepository.Update(user);
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
