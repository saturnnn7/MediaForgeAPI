using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Commands.UpdateChannel;

public sealed record UpdateChannelCommand(string Name, string? Description, string? AvatarUrl) : IRequest<Result<ChannelDto>>;
