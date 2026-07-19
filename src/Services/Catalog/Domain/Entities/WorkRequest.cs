namespace MediaForge.Catalog.Domain.Entities;

public sealed class WorkRequest : AggregateRoot
{
    private const int MaxTitleLength = 200;
    private const int MaxAuthorNamesLength = 500;
    private const int MaxDescriptionLength = 2000;
    private const int MaxCoverUrlLength = 2000;
    private const int MaxLanguageLength = 10;
    private const int MaxAdminNoteLength = 1000;

    private WorkRequest() { }

    public Guid RequesterId { get; init; }
    public WorkType WorkType { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? AuthorNames { get; private set; }
    public string? Description { get; private set; }
    public string? CoverUrl { get; private set; }
    public string? Language { get; private set; }
    public WorkRequestStatus Status { get; private set; } = WorkRequestStatus.Pending;
    public string? AdminNote { get; private set; }
    public Guid? ResultingWorkId { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ReviewedAt { get; private set; }

    public static Result<WorkRequest> Create(
        Guid requesterId,
        WorkType workType,
        string title,
        string? authorNames,
        string? description,
        string? coverUrl,
        string? language)
    {
        if (requesterId == Guid.Empty)
            return Result.Failure<WorkRequest>(Error.Validation("RequesterId", "RequesterId is required."));

        var validation = ValidateDetails(title, authorNames, description, coverUrl, language);
        if (validation.IsFailure)
            return Result.Failure<WorkRequest>(validation.Error);

        var request = new WorkRequest
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterId,
            WorkType = workType,
            Title = title,
            AuthorNames = authorNames,
            Description = description,
            CoverUrl = coverUrl,
            Language = language,
            Status = WorkRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        return Result.Success(request);
    }

    public Result Approve(Guid workId)
    {
        if (Status != WorkRequestStatus.Pending)
            return Result.Failure(Error.Conflict("WorkRequest", "This request has already been reviewed."));

        Status = WorkRequestStatus.Approved;
        ResultingWorkId = workId;
        ReviewedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result Reject(string? adminNote)
    {
        if (Status != WorkRequestStatus.Pending)
            return Result.Failure(Error.Conflict("WorkRequest", "This request has already been reviewed."));

        Status = WorkRequestStatus.Rejected;
        AdminNote = adminNote;
        ReviewedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static Result ValidateDetails(string title, string? authorNames, string? description, string? coverUrl, string? language)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Failure(Error.Validation("Title", "Title is required."));

        if (title.Length > MaxTitleLength)
            return Result.Failure(Error.Validation("Title", $"Title must not exceed {MaxTitleLength} characters."));

        if (authorNames is { Length: > MaxAuthorNamesLength })
            return Result.Failure(Error.Validation("AuthorNames", $"AuthorNames must not exceed {MaxAuthorNamesLength} characters."));

        if (description is { Length: > MaxDescriptionLength })
            return Result.Failure(Error.Validation("Description", $"Description must not exceed {MaxDescriptionLength} characters."));

        if (coverUrl is { Length: > MaxCoverUrlLength })
            return Result.Failure(Error.Validation("CoverUrl", $"CoverUrl must not exceed {MaxCoverUrlLength} characters."));

        if (language is { Length: > MaxLanguageLength })
            return Result.Failure(Error.Validation("Language", $"Language must not exceed {MaxLanguageLength} characters."));

        return Result.Success();
    }
}
