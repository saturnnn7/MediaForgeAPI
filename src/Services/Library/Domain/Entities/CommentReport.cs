namespace MediaForge.Library.Domain.Entities;

public sealed class CommentReport
{
    private const int MaxReasonLength = 500;

    private CommentReport() { }

    public Guid Id { get; init; }
    public Guid TargetId { get; init; }
    public ReactionTarget TargetType { get; init; }
    public Guid ReporterId { get; init; }
    public string Reason { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public bool IsResolved { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    public static CommentReport Create(Guid targetId, ReactionTarget targetType, Guid reporterId, string reason) =>
        new()
        {
            Id = Guid.NewGuid(),
            TargetId = targetId,
            TargetType = targetType,
            ReporterId = reporterId,
            Reason = reason.Length > MaxReasonLength ? reason[..MaxReasonLength] : reason,
            CreatedAt = DateTime.UtcNow
        };

    public Result Resolve()
    {
        if (IsResolved)
        {
            return Result.Failure(Error.Conflict("CommentReport", "Report is already resolved."));
        }

        IsResolved = true;
        ResolvedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
