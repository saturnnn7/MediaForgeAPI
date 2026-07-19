namespace MediaForge.Library.Domain.Entities;

public sealed class Review : AggregateRoot
{
    private const int MaxTextLength = 10000;

    private readonly List<ReviewComment> _comments = [];

    private Review() { }

    public Guid UserId { get; init; }
    public Guid WorkId { get; init; }
    public string Text { get; private set; } = string.Empty;
    public bool ContainsSpoiler { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? EditedAt { get; private set; }
    public bool IsHidden { get; private set; }
    public int ReportCount { get; private set; }

    public IReadOnlyList<ReviewComment> Comments => _comments.AsReadOnly();

    public static Result<Review> Create(Guid userId, Guid workId, string text, bool containsSpoiler)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure<Review>(Error.Validation("Text", "Text is required."));

        if (text.Length > MaxTextLength)
            return Result.Failure<Review>(Error.Validation("Text", $"Text must not exceed {MaxTextLength} characters."));

        var review = new Review
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WorkId = workId,
            Text = text,
            ContainsSpoiler = containsSpoiler,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(review);
    }

    public Result Edit(string newText, bool containsSpoiler)
    {
        if (string.IsNullOrWhiteSpace(newText))
            return Result.Failure(Error.Validation("Text", "Text is required."));

        if (newText.Length > MaxTextLength)
            return Result.Failure(Error.Validation("Text", $"Text must not exceed {MaxTextLength} characters."));

        Text = newText;
        ContainsSpoiler = containsSpoiler;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void Hide() => IsHidden = true;

    public void IncrementReportCount() => ReportCount++;
}
