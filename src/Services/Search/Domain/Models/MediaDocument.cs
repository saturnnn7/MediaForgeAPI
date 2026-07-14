namespace MediaForge.Search.Domain.Models;

public sealed class MediaDocument
{
    public string Id { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string? Description { get; init; }
    public string? TranscriptionText { get; init; }
    public string MediaType { get; init; } = default!;
    public string ThumbnailUrl { get; init; } = default!;
    public IReadOnlyList<string> OutputUrls { get; init; } = [];
    public double DurationSeconds { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime IndexedAt { get; init; }
}
