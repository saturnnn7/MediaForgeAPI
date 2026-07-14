using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace MediaForge.Media.IntegrationTests.Fixtures;

public sealed class MediaInfrastructureFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("media_test")
        .WithUsername("test")
        .WithPassword("test")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithImage("rabbitmq:3.13-alpine")
        .Build();

    private readonly MinioContainer _minio = new MinioBuilder()
        .WithImage("minio/minio:RELEASE.2024-11-07T00-52-20Z")
        .WithUsername("minioadmin")
        .WithPassword("minioadmin")
        .Build();

    public string PostgresConnectionString => _postgres.GetConnectionString();

    public string RabbitMqConnectionString => _rabbitMq.GetConnectionString();

    public string MinioEndpoint => $"http://{_minio.Hostname}:{_minio.GetMappedPublicPort(9000)}";

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_postgres.StartAsync(), _rabbitMq.StartAsync(), _minio.StartAsync());
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(_postgres.StopAsync(), _rabbitMq.StopAsync(), _minio.StopAsync());
    }
}

[CollectionDefinition("Media")]
public sealed class MediaCollection : ICollectionFixture<MediaInfrastructureFixture>;
