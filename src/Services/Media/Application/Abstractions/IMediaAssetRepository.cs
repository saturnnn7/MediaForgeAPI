namespace MediaForge.Media.Application.Abstractions;

public interface IMediaAssetRepository
{
    Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<MediaAsset>> GetByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    Task AddAsync(MediaAsset asset, CancellationToken ct);
    void Update(MediaAsset asset);
}
