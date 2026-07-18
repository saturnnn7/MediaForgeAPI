namespace MediaForge.Catalog.Domain.Entities;

public sealed class WorkGenre
{
    private WorkGenre() { }

    public Guid Id { get; init; }
    public Guid WorkId { get; init; }
    public Guid GenreId { get; init; }

    public static WorkGenre Create(Guid workId, Guid genreId) =>
        new()
        {
            Id = Guid.NewGuid(),
            WorkId = workId,
            GenreId = genreId
        };
}
