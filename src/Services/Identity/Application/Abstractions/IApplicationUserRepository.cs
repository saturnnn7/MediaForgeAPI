namespace MediaForge.Identity.Application.Abstractions;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ApplicationUser?> GetByIdWithChannelAsync(Guid id, CancellationToken ct);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task<IReadOnlyList<ApplicationUser>> GetPagedAsync(int page, int pageSize, UserRole? role, string? search, CancellationToken ct);
    Task<int> CountAsync(UserRole? role, string? search, CancellationToken ct);
    Task AddAsync(ApplicationUser user, CancellationToken ct);
    void Update(ApplicationUser user);
}
