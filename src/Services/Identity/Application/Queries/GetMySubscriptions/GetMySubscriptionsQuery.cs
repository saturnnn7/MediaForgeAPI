using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetMySubscriptions;

public sealed record GetMySubscriptionsQuery : IRequest<Result<IReadOnlyList<ChannelDto>>>;
