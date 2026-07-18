namespace MediaForge.Catalog.Application.Abstractions;

public interface ISeriesRepository
{
    Task<Series?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Series>> GetByChannelIdAsync(Guid channelId, int page, int pageSize, CancellationToken ct);
    Task AddAsync(Series series, CancellationToken ct);
    void Update(Series series);
}
