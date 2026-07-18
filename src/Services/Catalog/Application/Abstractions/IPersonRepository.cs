namespace MediaForge.Catalog.Application.Abstractions;

public interface IPersonRepository
{
    Task<Person?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Person>> SearchAsync(string query, int page, int pageSize, CancellationToken ct);
    Task AddAsync(Person person, CancellationToken ct);
    void Update(Person person);
}
