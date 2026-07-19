namespace MediaForge.Library.Application.Abstractions;

public interface ILibraryEntryRepository
{
    Task<LibraryEntry?> GetByUserAndWorkAsync(Guid userId, Guid workId, CancellationToken ct);
    Task<IReadOnlyList<LibraryEntry>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<LibraryEntry>> GetByWorkIdAsync(Guid workId, CancellationToken ct);
    Task AddAsync(LibraryEntry entry, CancellationToken ct);
    void Update(LibraryEntry entry);
    Task DeleteAsync(LibraryEntry entry, CancellationToken ct);
}
