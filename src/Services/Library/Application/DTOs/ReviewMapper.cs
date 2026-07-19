namespace MediaForge.Library.Application.DTOs;

public static class ReviewMapper
{
    public static IReadOnlyList<ReactionSummaryDto> ToSummaries(this IReadOnlyList<ReviewReaction> reactions, Guid currentUserId) =>
        reactions
            .GroupBy(r => r.Emoji)
            .Select(g => new ReactionSummaryDto(g.Key, g.Count(), g.Any(r => r.UserId == currentUserId)))
            .ToList();

    public static ReviewDto ToDto(this Review review, IReadOnlyList<ReviewReaction> reactions, Guid currentUserId) =>
        new(
            review.Id,
            review.UserId,
            review.WorkId,
            review.Text,
            review.ContainsSpoiler,
            review.IsEdited,
            review.CreatedAt,
            review.EditedAt,
            review.IsHidden,
            review.ReportCount,
            review.Comments.Count,
            reactions.ToSummaries(currentUserId));

    public static ReviewCommentDto ToDto(this ReviewComment comment, IReadOnlyList<ReviewReaction> reactions, Guid currentUserId) =>
        new(
            comment.Id,
            comment.ReviewId,
            comment.ParentCommentId,
            comment.UserId,
            comment.Text,
            comment.ContainsSpoiler,
            comment.IsEdited,
            comment.CreatedAt,
            comment.EditedAt,
            comment.IsHidden,
            comment.ReportCount,
            comment.Depth,
            reactions.ToSummaries(currentUserId));
}
