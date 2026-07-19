namespace MediaForge.Library.Infrastructure.Persistence.Repositories;

public sealed class LibraryEntryRepository(LibraryDbContext dbContext) : ILibraryEntryRepository
{
    public Task<LibraryEntry?> GetByUserAndWorkAsync(Guid userId, Guid workId, CancellationToken ct) =>
        dbContext.LibraryEntries.FirstOrDefaultAsync(x => x.UserId == userId && x.WorkId == workId, ct);

    public async Task<IReadOnlyList<LibraryEntry>> GetByUserIdAsync(Guid userId, CancellationToken ct) =>
        await dbContext.LibraryEntries
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<LibraryEntry>> GetByWorkIdAsync(Guid workId, CancellationToken ct) =>
        await dbContext.LibraryEntries
            .Where(x => x.WorkId == workId)
            .ToListAsync(ct);

    public async Task AddAsync(LibraryEntry entry, CancellationToken ct) =>
        await dbContext.LibraryEntries.AddAsync(entry, ct);

    public void Update(LibraryEntry entry) =>
        dbContext.LibraryEntries.Update(entry);

    public Task DeleteAsync(LibraryEntry entry, CancellationToken ct)
    {
        dbContext.LibraryEntries.Remove(entry);
        return Task.CompletedTask;
    }
}
