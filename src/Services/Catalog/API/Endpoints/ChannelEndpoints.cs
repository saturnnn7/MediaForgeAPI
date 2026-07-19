using MediaForge.Catalog.Application.Queries.GetChannelRssFeed;

namespace MediaForge.Catalog.API.Endpoints;

public static class ChannelEndpoints
{
    public static IEndpointRouteBuilder MapChannelEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/channels/{channelId:guid}/rss", async (
            Guid channelId,
            ISender sender,
            IConfiguration configuration,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetChannelRssFeedQuery(channelId), ct);
            if (result.IsFailure)
                return Results.NotFound();

            var channelUrl = $"{configuration["App:BaseUrl"]}/api/channels/{channelId}";
            var xml = RssFeedBuilder.Build(result.Value, channelUrl);
            return Results.Content(xml, "application/rss+xml; charset=utf-8");
        });

        return app;
    }
}
