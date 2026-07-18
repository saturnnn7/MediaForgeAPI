namespace MediaForge.Search.Domain.Models;

public sealed class PersonDocument
{
    public string Id { get; init; } = default!;
    public string DocumentType { get; init; } = "person";
    public string Name { get; init; } = default!;
    public string? Bio { get; init; }
    public string? PhotoUrl { get; init; }
    public DateTime IndexedAt { get; init; }
}
