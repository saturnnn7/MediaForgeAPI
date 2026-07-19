namespace MediaForge.Identity.Application.DTOs;

public sealed record AdminUserListDto(IReadOnlyList<AdminUserSummaryDto> Users, int TotalCount, int Page, int PageSize);
