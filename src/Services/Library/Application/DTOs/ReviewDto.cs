namespace MediaForge.Library.Application.DTOs;

public sealed record ReviewDto(
    Guid Id,
    Guid UserId,
    Guid WorkId,
    string Text,
    bool ContainsSpoiler,
    bool IsEdited,
    DateTime CreatedAt,
    DateTime? EditedAt,
    bool IsHidden,
    int ReportCount,
    int CommentCount,
    IReadOnlyList<ReactionSummaryDto> Reactions);
