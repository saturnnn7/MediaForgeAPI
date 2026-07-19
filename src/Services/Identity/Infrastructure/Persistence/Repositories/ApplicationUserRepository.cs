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

    public async Task<IReadOnlyList<ApplicationUser>> GetPagedAsync(int page, int pageSize, UserRole? role, string? search, CancellationToken ct) =>
        await FilterQuery(role, search)
            .OrderBy(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task<int> CountAsync(UserRole? role, string? search, CancellationToken ct) =>
        FilterQuery(role, search).CountAsync(ct);

    private IQueryable<ApplicationUser> FilterQuery(UserRole? role, string? search)
    {
        var query = dbContext.ApplicationUsers.AsQueryable();

        if (role is not null)
        {
            query = query.Where(u => u.Role == role);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.Email.Contains(search) || u.DisplayName.Contains(search));
        }

        return query;
    }

    public async Task AddAsync(ApplicationUser user, CancellationToken ct) =>
        await dbContext.ApplicationUsers.AddAsync(user, ct);

    public void Update(ApplicationUser user) =>
        dbContext.ApplicationUsers.Update(user);
}
