namespace MediaForge.Library.Domain.Entities;

public sealed class UserList : AggregateRoot
{
    private const int MaxNameLength = 100;
    private const int MaxSlugLength = 100;
    private const int MaxDescriptionLength = 500;
    private const int MaxAvatarUrlLength = 2000;

    private readonly List<UserListItem> _items = [];

    private UserList() { }

    public Guid UserId { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? AvatarUrl { get; private set; }
    public ListPrivacy Privacy { get; private set; } = ListPrivacy.Everyone;
    public bool IsSystem { get; init; }
    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<UserListItem> Items => _items.AsReadOnly();

    public static Result<UserList> Create(Guid userId, string name, string slug, bool isSystem = false)
    {
        if (userId == Guid.Empty)
            return Result.Failure<UserList>(Error.Validation("UserId", "UserId is required."));

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<UserList>(Error.Validation("Name", "Name is required."));

        if (name.Length > MaxNameLength)
            return Result.Failure<UserList>(Error.Validation("Name", $"Name must not exceed {MaxNameLength} characters."));

        if (string.IsNullOrWhiteSpace(slug))
            return Result.Failure<UserList>(Error.Validation("Slug", "Slug is required."));

        if (slug.Length > MaxSlugLength)
            return Result.Failure<UserList>(Error.Validation("Slug", $"Slug must not exceed {MaxSlugLength} characters."));

        var list = new UserList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Slug = slug,
            IsSystem = isSystem,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(list);
    }

    public Result UpdateDetails(string name, string? description, string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(Error.Validation("Name", "Name is required."));

        if (name.Length > MaxNameLength)
            return Result.Failure(Error.Validation("Name", $"Name must not exceed {MaxNameLength} characters."));

        if (description is { Length: > MaxDescriptionLength })
            return Result.Failure(Error.Validation("Description", $"Description must not exceed {MaxDescriptionLength} characters."));

        if (avatarUrl is { Length: > MaxAvatarUrlLength })
            return Result.Failure(Error.Validation("AvatarUrl", $"AvatarUrl must not exceed {MaxAvatarUrlLength} characters."));

        Name = name;
        Description = description;
        AvatarUrl = avatarUrl;

        return Result.Success();
    }

    public void SetPrivacy(ListPrivacy privacy) => Privacy = privacy;

    public Result<UserListItem> AddItem(Guid workId, int displayOrder)
    {
        if (_items.Any(i => i.WorkId == workId))
            return Result.Failure<UserListItem>(Error.Conflict("UserListItem", "This work is already in the list."));

        var item = UserListItem.Create(Id, workId, displayOrder);
        _items.Add(item);

        return Result.Success(item);
    }

    public Result RemoveItem(Guid workId)
    {
        var item = _items.FirstOrDefault(i => i.WorkId == workId);
        if (item is null)
            return Result.Failure(Error.NotFound("UserListItem", workId));

        _items.Remove(item);

        return Result.Success();
    }

    public Result ReorderItem(Guid workId, int newOrder)
    {
        var item = _items.FirstOrDefault(i => i.WorkId == workId);
        if (item is null)
            return Result.Failure(Error.NotFound("UserListItem", workId));

        item.Reorder(newOrder);

        return Result.Success();
    }
}
