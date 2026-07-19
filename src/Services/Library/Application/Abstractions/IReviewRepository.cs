namespace MediaForge.Library.Application.Abstractions;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Review?> GetByIdWithCommentsAsync(Guid id, CancellationToken ct);
    Task<Review?> GetByUserAndWorkAsync(Guid userId, Guid workId, CancellationToken ct);
    Task<IReadOnlyList<Review>> GetByWorkIdAsync(Guid workId, int page, int pageSize, CancellationToken ct);
    Task AddAsync(Review review, CancellationToken ct);
    void Update(Review review);
    Task AddCommentAsync(ReviewComment comment, CancellationToken ct);
    Task<ReviewComment?> GetCommentByIdAsync(Guid commentId, CancellationToken ct);
    Task AddReactionAsync(ReviewReaction reaction, CancellationToken ct);
    Task<ReviewReaction?> GetReactionAsync(Guid targetId, Guid userId, string emoji, CancellationToken ct);
    Task<IReadOnlyList<ReviewReaction>> GetReactionsAsync(Guid targetId, CancellationToken ct);
    Task RemoveReactionAsync(Guid targetId, Guid userId, string emoji, CancellationToken ct);
    Task AddReportAsync(CommentReport report, CancellationToken ct);
}
