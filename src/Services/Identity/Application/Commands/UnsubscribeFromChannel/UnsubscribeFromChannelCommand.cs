namespace MediaForge.Identity.Application.Commands.UnsubscribeFromChannel;

public sealed record UnsubscribeFromChannelCommand(Guid ChannelId) : IRequest<Result>;
