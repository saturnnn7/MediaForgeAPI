using Elastic.Clients.Elasticsearch;
using Testcontainers.Elasticsearch;

namespace MediaForge.Search.IntegrationTests.Fixtures;

public sealed class ElasticsearchFixture : IAsyncLifetime
{
    private readonly ElasticsearchContainer _elasticsearch = new ElasticsearchBuilder()
        .WithImage("docker.elastic.co/elasticsearch/elasticsearch:8.17.6")
        .WithEnvironment("xpack.security.enabled", "false")
        .Build();

    public ElasticsearchClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        await _elasticsearch.StartAsync();
        var url = $"http://{_elasticsearch.Hostname}:{_elasticsearch.GetMappedPublicPort(9200)}";
        Client = new ElasticsearchClient(new Uri(url));
    }

    public async Task DisposeAsync()
    {
        await _elasticsearch.StopAsync();
    }
}

[CollectionDefinition("Elasticsearch")]
public sealed class ElasticsearchCollection : ICollectionFixture<ElasticsearchFixture>;
