using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetMyChannel;

public sealed record GetMyChannelQuery : IRequest<Result<ChannelDto>>;
