namespace MediaForge.Catalog.Infrastructure.Persistence.Repositories;

public sealed class PersonRepository(CatalogDbContext dbContext) : IPersonRepository
{
    public Task<Person?> GetByIdAsync(Guid id, CancellationToken ct) =>
        dbContext.Persons.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<IReadOnlyList<Person>> SearchAsync(string query, int page, int pageSize, CancellationToken ct) =>
        await dbContext.Persons
            .Where(x => EF.Functions.ILike(x.Name, $"%{query}%"))
            .OrderBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task AddAsync(Person person, CancellationToken ct) =>
        await dbContext.Persons.AddAsync(person, ct);

    public void Update(Person person) =>
        dbContext.Persons.Update(person);
}
