using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MediaForge.Catalog.Infrastructure.Services;

public sealed class ExternalRatingFetcher(IHttpClientFactory httpClientFactory, ILogger<ExternalRatingFetcher> logger)
    : IExternalRatingFetcher
{
    public async Task<(double? Score, int? ReviewCount)?> FetchAsync(
        ExternalRatingSource source, string externalId, CancellationToken ct)
    {
        if (source != ExternalRatingSource.OpenLibrary)
        {
            logger.LogWarning("External rating fetch not implemented for {Source}", source);
            return null;
        }

        var client = httpClientFactory.CreateClient("external-ratings");

        using var response = await client.GetAsync($"https://openlibrary.org/works/{externalId}.json", ct);
        if (!response.IsSuccessStatusCode)
            return null;

        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var root = document.RootElement;

        double? score = root.TryGetProperty("ratings_average", out var scoreElement) && scoreElement.ValueKind == JsonValueKind.Number
            ? scoreElement.GetDouble()
            : null;

        int? reviewCount = root.TryGetProperty("ratings_count", out var countElement) && countElement.ValueKind == JsonValueKind.Number
            ? countElement.GetInt32()
            : null;

        return (score, reviewCount);
    }
}
