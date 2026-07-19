namespace MediaForge.Catalog.Application.Abstractions;

public interface IExternalRatingFetcher
{
    Task<(double? Score, int? ReviewCount)?> FetchAsync(
        ExternalRatingSource source, string externalId, CancellationToken ct);
}
