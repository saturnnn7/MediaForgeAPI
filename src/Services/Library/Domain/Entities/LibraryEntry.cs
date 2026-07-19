namespace MediaForge.Library.Domain.Entities;

public sealed class LibraryEntry : AggregateRoot
{
    private LibraryEntry() { }

    public Guid UserId { get; init; }
    public Guid WorkId { get; init; }
    public LibraryStatus Status { get; private set; }
    public bool IsFavorite { get; private set; }
    public int? Rating { get; private set; }
    public ListPrivacy Privacy { get; private set; } = ListPrivacy.Everyone;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; private set; }

    public static Result<LibraryEntry> Create(Guid userId, Guid workId, LibraryStatus status)
    {
        if (userId == Guid.Empty)
            return Result.Failure<LibraryEntry>(Error.Validation("UserId", "UserId is required."));

        if (workId == Guid.Empty)
            return Result.Failure<LibraryEntry>(Error.Validation("WorkId", "WorkId is required."));

        if (status == LibraryStatus.Favorites)
            return Result.Failure<LibraryEntry>(Error.Validation("Status", "Use ToggleFavorite to mark an entry as a favorite."));

        var entry = new LibraryEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkId = workId,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return Result.Success(entry);
    }

    public Result ChangeStatus(LibraryStatus newStatus)
    {
        if (newStatus == LibraryStatus.Favorites)
            return Result.Failure(Error.Validation("Status", "Use ToggleFavorite to mark an entry as a favorite."));

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result SetRating(int? rating)
    {
        if (rating is < 1 or > 10)
            return Result.Failure(Error.Validation("Rating", "Rating must be between 1 and 10."));

        Rating = rating;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void SetPrivacy(ListPrivacy privacy)
    {
        Privacy = privacy;
        UpdatedAt = DateTime.UtcNow;
    }
}
