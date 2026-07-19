namespace MediaForge.Catalog.Application.DTOs;

public sealed record ExternalRatingDto(
    Guid Id,
    Guid WorkId,
    string Source,
    string ExternalId,
    double? Score,
    int? ReviewCount,
    string? ExternalUrl,
    DateTime? LastFetchedAt);
