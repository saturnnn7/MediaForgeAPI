using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetAdminUserById;

public sealed record GetAdminUserByIdQuery(Guid UserId) : IRequest<Result<AdminUserDetailDto>>;
