using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetMyFollows;

public sealed record GetMyFollowsQuery : IRequest<Result<IReadOnlyList<AuthorFollowDto>>>;
