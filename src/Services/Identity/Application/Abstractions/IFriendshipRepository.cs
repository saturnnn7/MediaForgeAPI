namespace MediaForge.Identity.Application.Abstractions;

public interface IFriendshipRepository
{
    Task<Friendship?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Friendship?> GetBetweenUsersAsync(Guid userId1, Guid userId2, CancellationToken ct);
    Task<IReadOnlyList<Friendship>> GetFriendsAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Friendship>> GetPendingRequestsAsync(Guid userId, CancellationToken ct);
    Task AddAsync(Friendship friendship, CancellationToken ct);
    void Update(Friendship friendship);
    Task DeleteAsync(Friendship friendship, CancellationToken ct);
}
