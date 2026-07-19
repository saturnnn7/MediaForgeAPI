using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace MediaForge.Catalog.IntegrationTests.Fixtures;

public sealed class CatalogInfrastructureFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("catalog_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.13-alpine")
        .Build();

    public string PostgresConnectionString => _postgres.GetConnectionString();

    public string RabbitMqConnectionString => _rabbitMq.GetConnectionString();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgres.StartAsync(), _rabbitMq.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(_postgres.StopAsync(), _rabbitMq.StopAsync());
    }
}

[CollectionDefinition("Catalog")]
public sealed class CatalogCollection : ICollectionFixture<CatalogInfrastructureFixture>;
