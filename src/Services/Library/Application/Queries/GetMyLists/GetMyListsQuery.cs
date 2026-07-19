using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetMyLists;

public sealed record GetMyListsQuery : IRequest<Result<IReadOnlyList<UserListDto>>>;
