using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MediaForge.Search.Infrastructure.Search;

public sealed class ElasticsearchIndexInitializer(
    ElasticsearchClient client,
    ILogger<ElasticsearchIndexInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await ElasticsearchSearchService.EnsureIndexAsync(client, cancellationToken);
        logger.LogInformation("Elasticsearch index {IndexName} ensured.", ElasticsearchSearchService.IndexName);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
