namespace MediaForge.Identity.Infrastructure.Persistence.Repositories;

public sealed class FriendshipRepository(IdentityDbContext dbContext) : IFriendshipRepository
{
    public Task<Friendship?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Friendships.FirstOrDefaultAsync(f => f.Id == id, ct);

    public Task<Friendship?> GetBetweenUsersAsync(Guid userId1, Guid userId2, CancellationToken ct) =>
        dbContext.Friendships.FirstOrDefaultAsync(
            f => (f.RequesterId == userId1 && f.AddresseeId == userId2)
                 || (f.RequesterId == userId2 && f.AddresseeId == userId1), ct);

    public async Task<IReadOnlyList<Friendship>> GetFriendsAsync(Guid userId, CancellationToken ct) =>
        await dbContext.Friendships
            .Where(f => (f.RequesterId == userId || f.AddresseeId == userId) && f.Status == FriendshipStatus.Accepted)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Friendship>> GetPendingRequestsAsync(Guid userId, CancellationToken ct) =>
        await dbContext.Friendships
            .Where(f => f.AddresseeId == userId && f.Status == FriendshipStatus.Pending)
            .ToListAsync(ct);

    public async Task AddAsync(Friendship friendship, CancellationToken ct) =>
        await dbContext.Friendships.AddAsync(friendship, ct);

    public void Update(Friendship friendship) =>
        dbContext.Friendships.Update(friendship);

    public Task DeleteAsync(Friendship friendship, CancellationToken ct)
    {
        dbContext.Friendships.Remove(friendship);
        return Task.CompletedTask;
    }
}
