namespace MediaForge.Identity.Domain.Entities;

public sealed class AuthorFollow
{
    private AuthorFollow() { }

    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid PersonId { get; init; }
    public DateTime FollowedAt { get; init; }

    public static AuthorFollow Create(Guid userId, Guid personId) =>
        new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PersonId = personId,
            FollowedAt = DateTime.UtcNow
        };
}
