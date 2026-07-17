namespace MediaForge.Media.Domain.Entities;

public sealed class Chapter
{
    private const int MaxTitleLength = 200;

    private Chapter() { }

    public Guid Id { get; init; }
    public Guid AssetId { get; init; }
    public string Title { get; private set; } = string.Empty;
    public TimeSpan StartTime { get; private set; }
    public TimeSpan? EndTime { get; private set; }
    public int Order { get; private set; }

    public static Result<Chapter> Create(Guid assetId, string title, TimeSpan startTime, int order, TimeSpan? endTime = null)
    {
        var validation = Validate(title, startTime, order);
        if (validation.IsFailure)
            return Result.Failure<Chapter>(validation.Error);

        var chapter = new Chapter
        {
            Id = Guid.NewGuid(),
            AssetId = assetId,
            Title = title,
            StartTime = startTime,
            EndTime = endTime,
            Order = order
        };

        return Result.Success(chapter);
    }

    public Result Update(string title, TimeSpan startTime, TimeSpan? endTime)
    {
        var validation = Validate(title, startTime, Order);
        if (validation.IsFailure)
            return validation;

        Title = title;
        StartTime = startTime;
        EndTime = endTime;

        return Result.Success();
    }

    internal void ReorderTo(int order) => Order = order;

    private static Result Validate(string title, TimeSpan startTime, int order)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation("Title", "Title is required."));

        if (title.Length > MaxTitleLength)
            return Result.Failure(Error.Validation("Title", $"Title must not exceed {MaxTitleLength} characters."));

        if (startTime < TimeSpan.Zero)
            return Result.Failure(Error.Validation("StartTime", "Start time must be non-negative."));

        if (order < 1)
            return Result.Failure(Error.Validation("Order", "Order must be 1 or greater."));

        return Result.Success();
    }
}
