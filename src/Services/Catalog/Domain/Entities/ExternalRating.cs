namespace MediaForge.Catalog.Domain.Entities;

public sealed class ExternalRating
{
    private const int MaxExternalIdLength = 100;
    private const int MaxExternalUrlLength = 2000;

    private ExternalRating() { }

    public Guid Id { get; init; }
    public Guid WorkId { get; init; }
    public ExternalRatingSource Source { get; init; }
    public string ExternalId { get; private set; } = string.Empty;
    public double? Score { get; private set; }
    public int? ReviewCount { get; private set; }
    public string? ExternalUrl { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastFetchedAt { get; private set; }

    public static Result<ExternalRating> Create(Guid workId, ExternalRatingSource source, string externalId, string? externalUrl)
    {
        var validation = ValidateExternalId(externalId);
        if (validation.IsFailure)
            return Result.Failure<ExternalRating>(validation.Error);

        var rating = new ExternalRating
        {
            Id = Guid.NewGuid(),
            WorkId = workId,
            Source = source,
            ExternalId = externalId,
            ExternalUrl = externalUrl,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(rating);
    }

    public void UpdateScore(double? score, int? reviewCount)
    {
        Score = score;
        ReviewCount = reviewCount;
        LastFetchedAt = DateTime.UtcNow;
    }

    public Result UpdateExternalId(string externalId, string? externalUrl)
    {
        var validation = ValidateExternalId(externalId);
        if (validation.IsFailure)
            return validation;

        ExternalId = externalId;
        ExternalUrl = externalUrl;

        return Result.Success();
    }

    private static Result ValidateExternalId(string externalId)
    {
        if (string.IsNullOrWhiteSpace(externalId))
            return Result.Failure(Error.Validation("ExternalId", "ExternalId is required."));

        if (externalId.Length > MaxExternalIdLength)
            return Result.Failure(Error.Validation("ExternalId", $"ExternalId must not exceed {MaxExternalIdLength} characters."));

        return Result.Success();
    }
}
