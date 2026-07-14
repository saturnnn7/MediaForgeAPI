using MediaForge.Media.Infrastructure.Persistence;

namespace MediaForge.Media.Infrastructure.Persistence.Repositories;

public sealed class MediaAssetRepository(MediaDbContext dbContext) : IMediaAssetRepository
{
    public Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.MediaAssets.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<MediaAsset>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct) =>
        await dbContext.MediaAssets
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(MediaAsset asset, CancellationToken ct) =>
        await dbContext.MediaAssets.AddAsync(asset, ct);

    public void Update(MediaAsset asset) =>
        dbContext.MediaAssets.Update(asset);
}
