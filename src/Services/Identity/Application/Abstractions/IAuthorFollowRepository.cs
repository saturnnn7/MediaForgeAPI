namespace MediaForge.Identity.Application.Abstractions;

public interface IAuthorFollowRepository
{
    Task<AuthorFollow?> GetAsync(Guid userId, Guid personId, CancellationToken ct);
    Task<IReadOnlyList<AuthorFollow>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<AuthorFollow>> GetByPersonIdAsync(Guid personId, CancellationToken ct);
    Task AddAsync(AuthorFollow follow, CancellationToken ct);
    Task RemoveAsync(Guid userId, Guid personId, CancellationToken ct);
}
