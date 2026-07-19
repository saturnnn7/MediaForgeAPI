namespace MediaForge.Identity.Infrastructure.Persistence.Repositories;

public sealed class AuthorFollowRepository(IdentityDbContext dbContext) : IAuthorFollowRepository
{
    public Task<AuthorFollow?> GetAsync(Guid userId, Guid personId, CancellationToken ct) =>
        dbContext.AuthorFollows.FirstOrDefaultAsync(f => f.UserId == userId && f.PersonId == personId, ct);

    public async Task<IReadOnlyList<AuthorFollow>> GetByUserIdAsync(Guid userId, CancellationToken ct) =>
        await dbContext.AuthorFollows.Where(f => f.UserId == userId).ToListAsync(ct);

    public async Task<IReadOnlyList<AuthorFollow>> GetByPersonIdAsync(Guid personId, CancellationToken ct) =>
        await dbContext.AuthorFollows.Where(f => f.PersonId == personId).ToListAsync(ct);

    public async Task AddAsync(AuthorFollow follow, CancellationToken ct) =>
        await dbContext.AuthorFollows.AddAsync(follow, ct);

    public async Task RemoveAsync(Guid userId, Guid personId, CancellationToken ct)
    {
        var follow = await dbContext.AuthorFollows.FirstOrDefaultAsync(f => f.UserId == userId && f.PersonId == personId, ct);
        if (follow is not null)
        {
            dbContext.AuthorFollows.Remove(follow);
        }
    }
}
