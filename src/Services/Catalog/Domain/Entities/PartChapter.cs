namespace MediaForge.Catalog.Domain.Entities;

public sealed class PartChapter
{
    private const int MaxTitleLength = 200;

    private PartChapter() { }

    public Guid Id { get; init; }
    public Guid PartId { get; init; }
    public string Title { get; private set; } = string.Empty;
    public double StartTimeSeconds { get; private set; }
    public double EndTimeSeconds { get; private set; }
    public int Order { get; private set; }

    public static Result<PartChapter> Create(Guid partId, string title, double start, double end, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure<PartChapter>(Error.Validation("Title", "Title is required."));

        if (title.Length > MaxTitleLength)
            return Result.Failure<PartChapter>(Error.Validation("Title", $"Title must not exceed {MaxTitleLength} characters."));

        if (start >= end)
            return Result.Failure<PartChapter>(Error.Validation("StartTimeSeconds", "Start time must be before end time."));

        if (order < 1)
            return Result.Failure<PartChapter>(Error.Validation("Order", "Order must be 1 or greater."));

        var chapter = new PartChapter
        {
            Id = Guid.NewGuid(),
            PartId = partId,
            Title = title,
            StartTimeSeconds = start,
            EndTimeSeconds = end,
            Order = order
        };

        return Result.Success(chapter);
    }
}
