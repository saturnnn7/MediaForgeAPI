using MediaForge.Catalog.Application.DTOs;

namespace MediaForge.Catalog.Application.Queries.GetChannelRssFeed;

public sealed record GetChannelRssFeedQuery(Guid ChannelId) : IRequest<Result<RssFeedDto>>;
