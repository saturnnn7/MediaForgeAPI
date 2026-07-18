using MediaForge.Media.Domain.Enums;
using MediaForge.Media.Domain.Events;

namespace MediaForge.Media.Domain.Entities;

public sealed class MediaAsset : AggregateRoot
{
    private const long MaxFileSizeBytes = 10_737_418_240L;

    private readonly List<string> _outputUrls = [];
    private readonly List<Chapter> _chapters = [];

    private MediaAsset() { }

    public Guid UserId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSizeBytes { get; private set; }
    public MediaType MediaType { get; private set; }
    public MediaAssetStatus Status { get; private set; }
    public string BucketName { get; private set; } = string.Empty;
    public string ObjectKey { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; private set; }
    public string? WaveformUrl { get; private set; }
    public string? SubtitleUrl { get; private set; }
    public string? TranscriptionText { get; private set; }
    public double? DurationSeconds { get; private set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessingCompletedAt { get; private set; }
    public Guid? PartId { get; private set; }

    public IReadOnlyList<string> OutputUrls => _outputUrls.AsReadOnly();
    public IReadOnlyList<Chapter> Chapters => _chapters.AsReadOnly();

    public static Result<MediaAsset> Create(
        Guid userId,
        string fileName,
        string contentType,
        long fileSizeBytes,
        MediaType mediaType,
        string bucketName,
        string objectKey)
    {
        if (fileSizeBytes > MaxFileSizeBytes)
        {
            return Result.Failure<MediaAsset>(Error.Validation("FileSizeBytes", "File size exceeds the 10GB limit."));
        }

        var asset = new MediaAsset
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FileName = fileName,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            MediaType = mediaType,
            Status = MediaAssetStatus.PendingUpload,
            BucketName = bucketName,
            ObjectKey = objectKey,
            CreatedAt = DateTime.UtcNow
        };

        asset.RaiseDomainEvent(new MediaAssetCreatedDomainEvent(asset.Id, asset.UserId));

        return Result.Success(asset);
    }

    public Result ConfirmUpload()
    {
        if (Status != MediaAssetStatus.PendingUpload)
        {
            return Result.Failure(Error.Conflict("MediaAsset", "Asset is not pending upload."));
        }

        Status = MediaAssetStatus.Uploaded;
        RaiseDomainEvent(new MediaAssetUploadedDomainEvent(Id, UserId, BucketName, ObjectKey, ContentType, FileSizeBytes));

        return Result.Success();
    }

    public Result StartProcessing()
    {
        if (Status is not (MediaAssetStatus.Uploaded or MediaAssetStatus.ProcessingQueued))
        {
            return Result.Failure(Error.Conflict("MediaAsset", "Asset must be uploaded or queued before processing."));
        }

        Status = MediaAssetStatus.Processing;

        return Result.Success();
    }

    public Result CompleteProcessing(
        string thumbnailUrl,
        string? transcription,
        IEnumerable<string> outputUrls,
        double durationSeconds,
        string? waveformUrl = null,
        string? subtitleUrl = null)
    {
        if (Status != MediaAssetStatus.Processing && Status != MediaAssetStatus.Uploaded)
        {
            return Result.Failure(Error.Conflict("MediaAsset", "Asset is not currently processing."));
        }

        Status = MediaAssetStatus.Completed;
        ThumbnailUrl = thumbnailUrl;
        TranscriptionText = transcription;
        _outputUrls.Clear();
        _outputUrls.AddRange(outputUrls);
        DurationSeconds = durationSeconds;
        WaveformUrl = waveformUrl;
        SubtitleUrl = subtitleUrl;
        ProcessingCompletedAt = DateTime.UtcNow;

        RaiseDomainEvent(new MediaAssetProcessingCompletedDomainEvent(Id, UserId));

        return Result.Success();
    }

    public Result FailProcessing(string reason)
    {
        Status = MediaAssetStatus.Failed;
        RaiseDomainEvent(new MediaAssetProcessingFailedDomainEvent(Id, UserId, reason));

        return Result.Success();
    }

    public Result Cancel()
    {
        Status = MediaAssetStatus.Cancelled;

        return Result.Success();
    }

    public void SetPartId(Guid partId) => PartId = partId;

    public Result QueueForProcessing()
    {
        if (Status != MediaAssetStatus.Uploaded)
        {
            return Result.Failure(Error.Conflict("MediaAsset", "Asset must be in Uploaded status to queue for processing."));
        }

        Status = MediaAssetStatus.ProcessingQueued;

        return Result.Success();
    }

    public Result<Chapter> AddChapter(string title, TimeSpan startTime, int order, TimeSpan? endTime = null)
    {
        if (_chapters.Any(c => c.Order == order))
            return Result.Failure<Chapter>(Error.Conflict("Chapter", $"A chapter with order {order} already exists."));

        var chapterResult = Chapter.Create(Id, title, startTime, order, endTime);
        if (chapterResult.IsFailure)
            return chapterResult;

        _chapters.Add(chapterResult.Value);

        return Result.Success(chapterResult.Value);
    }

    public Result RemoveChapter(Guid chapterId)
    {
        var chapter = _chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter is null)
            return Result.Failure(Error.NotFound("Chapter", chapterId));

        _chapters.Remove(chapter);

        var remaining = _chapters.OrderBy(c => c.Order).ToList();
        for (var i = 0; i < remaining.Count; i++)
        {
            remaining[i].ReorderTo(i + 1);
        }

        return Result.Success();
    }
}
