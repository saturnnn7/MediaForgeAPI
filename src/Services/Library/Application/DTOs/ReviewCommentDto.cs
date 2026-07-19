namespace MediaForge.Library.Application.DTOs;

public sealed record ReviewCommentDto(
    Guid Id,
    Guid ReviewId,
    Guid? ParentCommentId,
    Guid UserId,
    string Text,
    bool ContainsSpoiler,
    bool IsEdited,
    DateTime CreatedAt,
    DateTime? EditedAt,
    bool IsHidden,
    int ReportCount,
    int Depth,
    IReadOnlyList<ReactionSummaryDto> Reactions);
