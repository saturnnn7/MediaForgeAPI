namespace MediaForge.Library.Infrastructure.Persistence.Repositories;

public sealed class UserListRepository(LibraryDbContext dbContext) : IUserListRepository
{
    public Task<UserList?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.UserLists.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<UserList?> GetByIdWithItemsAsync(Guid id, CancellationToken ct) =>
        dbContext.UserLists
            .Include("_items")
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<UserList?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct) =>
        dbContext.UserLists.FirstOrDefaultAsync(x => x.UserId == userId && x.Slug == slug, ct);

    public async Task<IReadOnlyList<UserList>> GetByUserIdAsync(Guid userId, CancellationToken ct) =>
        await dbContext.UserLists
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

    public async Task AddAsync(UserList list, CancellationToken ct) =>
        await dbContext.UserLists.AddAsync(list, ct);

    public void Update(UserList list) =>
        dbContext.UserLists.Update(list);

    public async Task AddItemAsync(UserListItem item, CancellationToken ct) =>
        await dbContext.UserListItems.AddAsync(item, ct);

    public async Task RemoveItemAsync(Guid listId, Guid workId, CancellationToken ct)
    {
        var item = await dbContext.UserListItems
            .FirstOrDefaultAsync(x => x.ListId == listId && x.WorkId == workId, ct);

        if (item is not null)
            dbContext.UserListItems.Remove(item);
    }
}
