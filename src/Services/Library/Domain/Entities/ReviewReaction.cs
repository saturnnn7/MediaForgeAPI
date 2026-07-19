namespace MediaForge.Library.Domain.Entities;

public sealed class ReviewReaction
{
    private const int MaxEmojiLength = 10;

    private ReviewReaction() { }

    public Guid Id { get; init; }
    public Guid TargetId { get; init; }
    public ReactionTarget TargetType { get; init; }
    public Guid UserId { get; init; }
    public string Emoji { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    public static ReviewReaction Create(Guid targetId, ReactionTarget targetType, Guid userId, string emoji) =>
        new()
        {
            Id = Guid.NewGuid(),
            TargetId = targetId,
            TargetType = targetType,
            UserId = userId,
            Emoji = emoji.Length > MaxEmojiLength ? emoji[..MaxEmojiLength] : emoji,
            CreatedAt = DateTime.UtcNow
        };
}
