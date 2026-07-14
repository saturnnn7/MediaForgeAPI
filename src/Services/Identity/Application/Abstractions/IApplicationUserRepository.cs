namespace MediaForge.Identity.Application.Abstractions;

public interface IApplicationUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);
    Task AddAsync(ApplicationUser user, CancellationToken ct);
    void Update(ApplicationUser user);
}
