namespace MediaForge.Shared.Contracts.Events.Catalog;

public sealed record WorkPublishedEvent(
    Guid Id,
    DateTime OccurredOn,
    Guid CorrelationId,
    Guid WorkId,
    Guid ChannelId,
    Guid? SeriesId,
    string Title,
    string? Description,
    string WorkType,
    string? Language,
    string? CoverUrl,
    IReadOnlyList<string> ContributorNames,
    IReadOnlyList<string> GenreNames
) : IIntegrationEvent;
