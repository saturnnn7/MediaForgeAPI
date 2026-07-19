using MediaForge.Library.Application.DTOs;

namespace MediaForge.Library.Application.Queries.GetList;

public sealed record GetListQuery(Guid ListId) : IRequest<Result<UserListDetailDto>>;
