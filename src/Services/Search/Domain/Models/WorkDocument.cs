namespace MediaForge.Search.Domain.Models;

public sealed class WorkDocument
{
    public string Id { get; init; } = default!;
    public string DocumentType { get; init; } = "work";
    public string ChannelId { get; init; } = default!;
    public string? SeriesId { get; init; }
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public string WorkType { get; init; } = default!;
    public string? Language { get; init; }
    public string? CoverUrl { get; init; }
    public IReadOnlyList<string> ContributorNames { get; init; } = [];
    public IReadOnlyList<string> GenreNames { get; init; } = [];
    public DateTime IndexedAt { get; init; }
}
