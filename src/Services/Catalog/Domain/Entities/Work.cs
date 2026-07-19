namespace MediaForge.Catalog.Domain.Entities;

public sealed class Work : AggregateRoot
{
    private const int MaxTitleLength = 200;
    private const int MaxDescriptionLength = 2000;
    private const int MaxCoverUrlLength = 2000;
    private const int MaxLanguageLength = 10;

    private readonly List<WorkContributor> _contributors = [];
    private readonly List<WorkGenre> _genres = [];

    private Work() { }

    public Guid ChannelId { get; init; }
    public Guid CreatorId { get; private init; }
    public Guid? SeriesId { get; private set; }
    public WorkType WorkType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? CoverUrl { get; private set; }
    public string? Language { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public bool IsPublished { get; private set; }
    public bool IsPrivate { get; private set; }
    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<WorkContributor> Contributors => _contributors.AsReadOnly();
    public IReadOnlyList<WorkGenre> Genres => _genres.AsReadOnly();

    public static Result<Work> Create(Guid channelId, Guid creatorId, WorkType workType, string title)
    {
        var validation = ValidateDetails(title);
        if (validation.IsFailure)
            return Result.Failure<Work>(validation.Error);

        var work = new Work
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            CreatorId = creatorId,
            WorkType = workType,
            Title = title,
            IsPublished = false,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(work);
    }

    public Result UpdateDetails(string title, string? description, string? coverUrl, string? language)
    {
        var validation = ValidateDetails(title, description, coverUrl, language);
        if (validation.IsFailure)
            return validation;

        Title = title;
        Description = description;
        CoverUrl = coverUrl;
        Language = language;

        return Result.Success();
    }

    public Result Publish()
    {
        if (IsPublished)
            return Result.Failure(Error.Conflict("Work", "Work is already published."));

        IsPublished = true;
        PublishedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public void Unpublish()
    {
        IsPublished = false;
        PublishedAt = null;
    }

    public void MakePrivate() => IsPrivate = true;

    public void MakePublic() => IsPrivate = false;

    public void AssignToSeries(Guid seriesId) => SeriesId = seriesId;

    public void RemoveFromSeries() => SeriesId = null;

    public Result AddContributor(Guid personId, ContributorRole role)
    {
        if (_contributors.Any(c => c.PersonId == personId && c.Role == role))
            return Result.Failure(Error.Conflict("WorkContributor", "This person already has this role on the work."));

        _contributors.Add(WorkContributor.Create(Id, personId, role));

        return Result.Success();
    }

    public Result RemoveContributor(Guid personId, ContributorRole role)
    {
        var contributor = _contributors.FirstOrDefault(c => c.PersonId == personId && c.Role == role);
        if (contributor is null)
            return Result.Failure(Error.NotFound("WorkContributor", personId));

        _contributors.Remove(contributor);

        return Result.Success();
    }

    public Result AddGenre(Guid genreId)
    {
        if (_genres.Any(g => g.GenreId == genreId))
            return Result.Failure(Error.Conflict("WorkGenre", "This genre is already assigned to the work."));

        _genres.Add(WorkGenre.Create(Id, genreId));

        return Result.Success();
    }

    public Result RemoveGenre(Guid genreId)
    {
        var genre = _genres.FirstOrDefault(g => g.GenreId == genreId);
        if (genre is null)
            return Result.Failure(Error.NotFound("WorkGenre", genreId));

        _genres.Remove(genre);

        return Result.Success();
    }

    private static Result ValidateDetails(string title, string? description = null, string? coverUrl = null, string? language = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation("Title", "Title is required."));

        if (title.Length > MaxTitleLength)
            return Result.Failure(Error.Validation("Title", $"Title must not exceed {MaxTitleLength} characters."));

        if (description is { Length: > MaxDescriptionLength })
            return Result.Failure(Error.Validation("Description", $"Description must not exceed {MaxDescriptionLength} characters."));

        if (coverUrl is { Length: > MaxCoverUrlLength })
            return Result.Failure(Error.Validation("CoverUrl", $"CoverUrl must not exceed {MaxCoverUrlLength} characters."));

        if (language is { Length: > MaxLanguageLength })
            return Result.Failure(Error.Validation("Language", $"Language must not exceed {MaxLanguageLength} characters."));

        return Result.Success();
    }
}
