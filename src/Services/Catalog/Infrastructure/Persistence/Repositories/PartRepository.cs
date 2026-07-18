namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class PartRepository(CatalogDbContext dbContext) : IPartRepository
{
    public Task<Part?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Parts.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Part?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct) =>
        dbContext.Parts
            .Include("_chapters")
            .Include("_assets")
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Part>> GetByWorkIdAsync(Guid workId, CancellationToken ct) =>
        await dbContext.Parts
            .Where(x => x.WorkId == workId)
            .OrderBy(x => x.OrderMajor).ThenBy(x => x.OrderMinor)
            .ToListAsync(ct);

    public async Task AddAsync(Part part, CancellationToken ct) =>
        await dbContext.Parts.AddAsync(part, ct);

    public void Update(Part part) =>
        dbContext.Parts.Update(part);

    public async Task AddChapterAsync(PartChapter chapter, CancellationToken ct) =>
        await dbContext.PartChapters.AddAsync(chapter, ct);

    public async Task AddAssetAsync(PartAsset asset, CancellationToken ct) =>
        await dbContext.PartAssets.AddAsync(asset, ct);

    public async Task RemoveChapterAsync(Guid chapterId, CancellationToken ct)
    {
        var chapter = await dbContext.PartChapters.FirstOrDefaultAsync(x => x.Id == chapterId, ct);
        if (chapter is not null)
            dbContext.PartChapters.Remove(chapter);
    }

    public async Task RemoveAssetAsync(Guid partId, Guid mediaAssetId, CancellationToken ct)
    {
        var asset = await dbContext.PartAssets
            .FirstOrDefaultAsync(x => x.PartId == partId && x.MediaAssetId == mediaAssetId, ct);

        if (asset is not null)
            dbContext.PartAssets.Remove(asset);
    }
}
