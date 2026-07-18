namespace MediaForge.Catalog.Domain.Entities;

public sealed class Series : AggregateRoot
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;
    private const int MaxCoverUrlLength = 2000;

    private Series() { }

    public Guid ChannelId { get; init; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? CoverUrl { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime CreatedAt { get; init; }

    public static Result<Series> Create(Guid channelId, string title)
    {
        var validation = Validate(title);
        if (validation.IsFailure)
            return Result.Failure<Series>(validation.Error);

        var series = new Series
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            Title = title,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(series);
    }

    public Result UpdateDetails(string title, string? description, string? coverUrl)
    {
        var validation = Validate(title, description, coverUrl);
        if (validation.IsFailure)
            return validation;

        Title = title;
        Description = description;
        CoverUrl = coverUrl;

        return Result.Success();
    }

    public void MarkCompleted() => IsCompleted = true;

    public void MarkOngoing() => IsCompleted = false;

    private static Result Validate(string title, string? description = null, string? coverUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation("Title", "Title is required."));

        if (title.Length > MaxTitleLength)
            return Result.Failure(Error.Validation("Title", $"Title must not exceed {MaxTitleLength} characters."));

        if (description is { Length: > MaxDescriptionLength })
            return Result.Failure(Error.Validation("Description", $"Description must not exceed {MaxDescriptionLength} characters."));

        if (coverUrl is { Length: > MaxCoverUrlLength })
            return Result.Failure(Error.Validation("CoverUrl", $"CoverUrl must not exceed {MaxCoverUrlLength} characters."));

        return Result.Success();
    }
}
