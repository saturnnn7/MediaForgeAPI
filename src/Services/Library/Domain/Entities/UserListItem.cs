namespace MediaForge.Library.Domain.Entities;

public sealed class UserListItem
{
    private UserListItem() { }

    public Guid Id { get; init; }
    public Guid ListId { get; init; }
    public Guid WorkId { get; init; }
    public int DisplayOrder { get; private set; }
    public DateTime AddedAt { get; init; }

    public static UserListItem Create(Guid listId, Guid workId, int displayOrder) =>
        new()
        {
            Id = Guid.NewGuid(),
            ListId = listId,
            WorkId = workId,
            DisplayOrder = displayOrder,
            AddedAt = DateTime.UtcNow
        };

    internal void Reorder(int newOrder) => DisplayOrder = newOrder;
}
