using MediaForge.Identity.Application.DTOs;

namespace MediaForge.Identity.Application.Queries.GetAdminUsers;

public sealed record GetAdminUsersQuery(int Page, int PageSize, UserRole? Role, string? Search) : IRequest<Result<AdminUserListDto>>;
