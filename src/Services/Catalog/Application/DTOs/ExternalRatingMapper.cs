namespace MediaForge.Catalog.Application.DTOs;

public static class ExternalRatingMapper
{
    public static ExternalRatingDto ToDto(this ExternalRating rating) =>
        new(
            rating.Id,
            rating.WorkId,
            rating.Source.ToString(),
            rating.ExternalId,
            rating.Score,
            rating.ReviewCount,
            rating.ExternalUrl,
            rating.LastFetchedAt);
}
