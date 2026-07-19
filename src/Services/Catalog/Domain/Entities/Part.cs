namespace MediaForge.Catalog.Domain.Entities;

public sealed class Part : AggregateRoot
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 1000;
    private const int MaxCoverUrlLength = 2000;

    private readonly List<PartChapter> _chapters = [];
    private readonly List<PartAsset> _assets = [];

    private Part() { }

    public Guid WorkId { get; init; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int OrderMajor { get; private set; }
    public int OrderMinor { get; private set; }
    public PartType PartType { get; private set; }
    public string? CoverUrl { get; private set; }
    public double? DurationSeconds { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsPrivate { get; private set; }
    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<PartChapter> Chapters => _chapters.AsReadOnly();
    public IReadOnlyList<PartAsset> Assets => _assets.AsReadOnly();

    public static Result<Part> Create(Guid workId, string title, int orderMajor, int orderMinor = 0, PartType partType = PartType.Regular)
    {
        var validation = ValidateDetails(title);
        if (validation.IsFailure)
            return Result.Failure<Part>(validation.Error);

        var part = new Part
        {
            Id = Guid.NewGuid(),
            WorkId = workId,
            Title = title,
            OrderMajor = orderMajor,
            OrderMinor = orderMinor,
            PartType = partType,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(part);
    }

    public Result UpdateDetails(string title, string? description, string? coverUrl)
    {
        var validation = ValidateDetails(title, description, coverUrl);
        if (validation.IsFailure)
            return validation;

        Title = title;
        Description = description;
        CoverUrl = coverUrl;

        return Result.Success();
    }

    public void Publish() => IsPublished = true;

    public void Unpublish() => IsPublished = false;

    public void MakePrivate() => IsPrivate = true;

    public void MakePublic() => IsPrivate = false;

    public Result<PartChapter> AddChapter(string title, double startTimeSeconds, double endTimeSeconds, int order)
    {
        var chapterResult = PartChapter.Create(Id, title, startTimeSeconds, endTimeSeconds, order);
        if (chapterResult.IsFailure)
            return chapterResult;

        _chapters.Add(chapterResult.Value);

        return Result.Success(chapterResult.Value);
    }

    public Result RemoveChapter(Guid chapterId)
    {
        var chapter = _chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter is null)
            return Result.Failure(Error.NotFound("PartChapter", chapterId));

        _chapters.Remove(chapter);

        return Result.Success();
    }

    public Result<PartAsset> LinkAsset(Guid mediaAssetId, int sequenceOrder)
    {
        if (_assets.Any(a => a.MediaAssetId == mediaAssetId))
            return Result.Failure<PartAsset>(Error.Conflict("PartAsset", "This media asset is already linked to the part."));

        var asset = PartAsset.Create(Id, mediaAssetId, sequenceOrder);
        _assets.Add(asset);

        return Result.Success(asset);
    }

    public Result UnlinkAsset(Guid mediaAssetId)
    {
        var asset = _assets.FirstOrDefault(a => a.MediaAssetId == mediaAssetId);
        if (asset is null)
            return Result.Failure(Error.NotFound("PartAsset", mediaAssetId));

        _assets.Remove(asset);

        return Result.Success();
    }

    public void SetDuration(double seconds) => DurationSeconds = seconds;

    private static Result ValidateDetails(string title, string? description = null, string? coverUrl = null)
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
