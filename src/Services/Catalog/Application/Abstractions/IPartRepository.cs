namespace MediaForge.Catalog.Application.Abstractions;

public interface IPartRepository
{
    Task<Part?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Part?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Part>> GetByWorkIdAsync(Guid workId, CancellationToken ct);
    Task AddAsync(Part part, CancellationToken ct);
    void Update(Part part);
    Task AddChapterAsync(PartChapter chapter, CancellationToken ct);
    Task AddAssetAsync(PartAsset asset, CancellationToken ct);
    Task RemoveChapterAsync(Guid chapterId, CancellationToken ct);
    Task RemoveAssetAsync(Guid partId, Guid mediaAssetId, CancellationToken ct);
}
