using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.CreateChannel;

public sealed record CreateChannelCommand(string Name) : IRequest<Result<ChannelDto>>;
