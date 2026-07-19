namespace MediaForge.Identity.Domain.Entities;

public sealed class Friendship
{
    private Friendship() { }

    public Guid Id { get; init; }
    public Guid RequesterId { get; init; }
    public Guid AddresseeId { get; init; }
    public FriendshipStatus Status { get; private set; } = FriendshipStatus.Pending;
    public DateTime CreatedAt { get; init; }
    public DateTime? RespondedAt { get; private set; }

    public static Result<Friendship> Create(Guid requesterId, Guid addresseeId)
    {
        if (requesterId == addresseeId)
        {
            return Result.Failure<Friendship>(Error.Validation("AddresseeId", "You cannot send a friend request to yourself."));
        }

        return Result.Success(new Friendship
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterId,
            AddresseeId = addresseeId,
            Status = FriendshipStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });
    }

    public Result Accept()
    {
        if (Status != FriendshipStatus.Pending)
        {
            return Result.Failure(Error.Conflict("Friendship", "This friend request has already been responded to."));
        }

        Status = FriendshipStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
        return Result.Success();
    }

    public Result Decline()
    {
        if (Status != FriendshipStatus.Pending)
        {
            return Result.Failure(Error.Conflict("Friendship", "This friend request has already been responded to."));
        }

        Status = FriendshipStatus.Declined;
        RespondedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
