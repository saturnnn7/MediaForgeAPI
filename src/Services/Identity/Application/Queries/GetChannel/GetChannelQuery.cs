using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetChannel;

public sealed record GetChannelQuery(Guid ChannelId) : IRequest<Result<ChannelDto>>;
