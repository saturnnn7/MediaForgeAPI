namespace MediaForge.Library.Application.Abstractions;

public interface IUserListRepository
{
    Task<UserList?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<UserList?> GetByIdWithItemsAsync(Guid id, CancellationToken ct);
    Task<UserList?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct);
    Task<IReadOnlyList<UserList>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(UserList list, CancellationToken ct);
    void Update(UserList list);
    Task AddItemAsync(UserListItem item, CancellationToken ct);
    Task RemoveItemAsync(Guid listId, Guid workId, CancellationToken ct);
}
