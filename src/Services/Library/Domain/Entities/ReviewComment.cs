namespace MediaForge.Library.Domain.Entities;

public sealed class ReviewComment
{
    private const int MaxTextLength = 5000;
    private const int MaxDepth = 2;

    private ReviewComment() { }

    public Guid Id { get; init; }
    public Guid ReviewId { get; init; }
    public Guid? ParentCommentId { get; init; }
    public Guid UserId { get; init; }
    public string Text { get; private set; } = string.Empty;
    public bool ContainsSpoiler { get; private set; }
    public bool IsEdited { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? EditedAt { get; private set; }
    public bool IsHidden { get; private set; }
    public int ReportCount { get; private set; }
    public int Depth { get; init; }

    public static Result<ReviewComment> Create(Guid reviewId, Guid? parentCommentId, Guid userId, string text, bool containsSpoiler, int depth)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.Failure<ReviewComment>(Error.Validation("Text", "Text is required."));

        if (text.Length > MaxTextLength)
            return Result.Failure<ReviewComment>(Error.Validation("Text", $"Text must not exceed {MaxTextLength} characters."));

        if (depth > MaxDepth)
            return Result.Failure<ReviewComment>(Error.Validation("Depth", $"Comment depth must not exceed {MaxDepth}."));

        var comment = new ReviewComment
        {
            Id = Guid.NewGuid(),
            ReviewId = reviewId,
            ParentCommentId = parentCommentId,
            UserId = userId,
            Text = text,
            ContainsSpoiler = containsSpoiler,
            Depth = depth,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(comment);
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
