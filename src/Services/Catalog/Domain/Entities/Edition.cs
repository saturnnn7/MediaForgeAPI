namespace MediaForge.Catalog.Domain.Entities;

public sealed class Edition : AggregateRoot
{
    private const int MaxNarratorTeamNameLength = 200;
    private const int MaxDescriptionLength = 1000;
    private const int MaxCoverUrlLength = 2000;
    private const int MaxLanguageLength = 10;

    private readonly List<Part> _parts = [];

    private Edition() { }

    public Guid WorkId { get; init; }
    public Guid CreatorId { get; init; }
    public string NarratorTeamName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? CoverUrl { get; private set; }
    public string Language { get; private set; } = "en";
    public bool IsDefault { get; private set; }
    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<Part> Parts => _parts.AsReadOnly();

    public static Result<Edition> Create(Guid workId, Guid creatorId, string narratorTeamName, string language = "en")
    {
        if (workId == Guid.Empty)
            return Result.Failure<Edition>(Error.Validation("WorkId", "WorkId is required."));

        if (creatorId == Guid.Empty)
            return Result.Failure<Edition>(Error.Validation("CreatorId", "CreatorId is required."));

        var validation = ValidateDetails(narratorTeamName, language: language);
        if (validation.IsFailure)
            return Result.Failure<Edition>(validation.Error);

        var edition = new Edition
        {
            Id = Guid.NewGuid(),
            WorkId = workId,
            CreatorId = creatorId,
            NarratorTeamName = narratorTeamName,
            Language = language,
            IsDefault = false,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(edition);
    }

    public Result UpdateDetails(string narratorTeamName, string? description, string? coverUrl, string? language)
    {
        var validation = ValidateDetails(narratorTeamName, description, coverUrl, language ?? Language);
        if (validation.IsFailure)
            return validation;

        NarratorTeamName = narratorTeamName;
        Description = description;
        CoverUrl = coverUrl;
        Language = language ?? Language;

        return Result.Success();
    }

    public void SetAsDefault() => IsDefault = true;

    public void UnsetDefault() => IsDefault = false;

    private static Result ValidateDetails(string narratorTeamName, string? description = null, string? coverUrl = null, string? language = null)
    {
        if (string.IsNullOrWhiteSpace(narratorTeamName))
            return Result.Failure(Error.Validation("NarratorTeamName", "NarratorTeamName is required."));

        if (narratorTeamName.Length > MaxNarratorTeamNameLength)
            return Result.Failure(Error.Validation("NarratorTeamName", $"NarratorTeamName must not exceed {MaxNarratorTeamNameLength} characters."));

        if (description is { Length: > MaxDescriptionLength })
            return Result.Failure(Error.Validation("Description", $"Description must not exceed {MaxDescriptionLength} characters."));

        if (coverUrl is { Length: > MaxCoverUrlLength })
            return Result.Failure(Error.Validation("CoverUrl", $"CoverUrl must not exceed {MaxCoverUrlLength} characters."));

        if (language is { Length: > MaxLanguageLength })
            return Result.Failure(Error.Validation("Language", $"Language must not exceed {MaxLanguageLength} characters."));

        return Result.Success();
    }
}
