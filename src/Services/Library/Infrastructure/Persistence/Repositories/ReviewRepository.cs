namespace MediaForge.Library.Infrastructure.Persistence.Repositories;

public sealed class ReviewRepository(LibraryDbContext dbContext) : IReviewRepository
{
    public Task<Review?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Review?> GetByIdWithCommentsAsync(Guid id, CancellationToken ct) =>
        dbContext.Reviews
            .Include("_comments")
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Review?> GetByUserAndWorkAsync(Guid userId, Guid workId, CancellationToken ct) =>
        dbContext.Reviews.FirstOrDefaultAsync(x => x.UserId == userId && x.WorkId == workId, ct);

    public async Task<IReadOnlyList<Review>> GetByWorkIdAsync(Guid workId, int page, int pageSize, CancellationToken ct) =>
        await dbContext.Reviews
            .Include("_comments")
            .Where(x => x.WorkId == workId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(Review review, CancellationToken ct) =>
        await dbContext.Reviews.AddAsync(review, ct);

    public void Update(Review review) =>
        dbContext.Reviews.Update(review);

    public async Task AddCommentAsync(ReviewComment comment, CancellationToken ct) =>
        await dbContext.ReviewComments.AddAsync(comment, ct);

    public Task<ReviewComment?> GetCommentByIdAsync(Guid commentId, CancellationToken ct) =>
        dbContext.ReviewComments.FirstOrDefaultAsync(x => x.Id == commentId, ct);

    public async Task AddReactionAsync(ReviewReaction reaction, CancellationToken ct) =>
        await dbContext.ReviewReactions.AddAsync(reaction, ct);

    public Task<ReviewReaction?> GetReactionAsync(Guid targetId, Guid userId, string emoji, CancellationToken ct) =>
        dbContext.ReviewReactions.FirstOrDefaultAsync(
            x => x.TargetId == targetId && x.UserId == userId && x.Emoji == emoji, ct);

    public async Task<IReadOnlyList<ReviewReaction>> GetReactionsAsync(Guid targetId, CancellationToken ct) =>
        await dbContext.ReviewReactions
            .Where(x => x.TargetId == targetId)
            .ToListAsync(ct);

    public async Task RemoveReactionAsync(Guid targetId, Guid userId, string emoji, CancellationToken ct)
    {
        var reaction = await dbContext.ReviewReactions
            .FirstOrDefaultAsync(x => x.TargetId == targetId && x.UserId == userId && x.Emoji == emoji, ct);

        if (reaction is not null)
            dbContext.ReviewReactions.Remove(reaction);
    }

    public async Task AddReportAsync(CommentReport report, CancellationToken ct) =>
        await dbContext.CommentReports.AddAsync(report, ct);
}
