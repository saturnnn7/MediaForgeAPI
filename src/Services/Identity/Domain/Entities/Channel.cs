namespace MediaForge.Identity.Domain.Entities;

public sealed class Channel : AggregateRoot
{
    private Channel() { }

    public Guid OwnerId { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? AvatarUrl { get; private set; }
    public DateTime CreatedAt { get; init; }
    public int SubscriberCount { get; private set; }

    public static Result<Channel> Create(Guid ownerId, string name)
    {
        if (ownerId == Guid.Empty)
        {
            return Result.Failure<Channel>(Error.Validation("OwnerId", "OwnerId must not be empty."));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Channel>(Error.Validation("Name", "Channel name must not be empty."));
        }

        var channel = new Channel
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(channel);
    }

    public Result UpdateProfile(string name, string? description, string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(Error.Validation("Name", "Channel name must not be empty."));
        }

        Name = name;
        Description = description;
        AvatarUrl = avatarUrl;

        return Result.Success();
    }

    public void IncrementSubscribers() => SubscriberCount++;

    public void DecrementSubscribers() => SubscriberCount = Math.Max(0, SubscriberCount - 1);
}
