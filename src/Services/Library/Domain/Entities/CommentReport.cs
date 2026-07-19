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
}
