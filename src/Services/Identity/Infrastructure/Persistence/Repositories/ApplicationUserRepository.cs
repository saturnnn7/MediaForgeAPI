using MediaForge.Identity.Infrastructure.Persistence;

namespace MediaForge.Identity.Infrastructure.Persistence.Repositories;

public sealed class ApplicationUserRepository(IdentityDbContext dbContext) : IApplicationUserRepository
{
    public Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<ApplicationUser?> GetByIdWithChannelAsync(Guid id, CancellationToken ct) =>
        dbContext.ApplicationUsers.Include(u => u.Channel).FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct) =>
        dbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct) =>
        dbContext.ApplicationUsers.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(ApplicationUser user, CancellationToken ct) =>
        await dbContext.ApplicationUsers.AddAsync(user, ct);

    public void Update(ApplicationUser user) =>
        dbContext.ApplicationUsers.Update(user);
}
