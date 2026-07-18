namespace MediaForge.Identity.Application.Commands.SubscribeToChannel;

public sealed record SubscribeToChannelCommand(Guid ChannelId) : IRequest<Result>;
