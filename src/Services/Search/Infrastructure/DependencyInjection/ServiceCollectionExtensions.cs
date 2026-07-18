using Elastic.Clients.Elasticsearch;
using MassTransit;
using MediaForge.Search.Application.Abstractions;
using MediaForge.Search.Application.Consumers;
using MediaForge.Search.Infrastructure.Search;
using MediaForge.Shared.Infrastructure.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaForge.Search.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSearchInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration["Search:Provider"] ?? "inmemory";

        var healthChecksBuilder = services.AddHealthChecks()
            .AddRabbitMqCheck(configuration.GetConnectionString("RabbitMq")!, tags: ["messaging"]);

        if (provider == "elasticsearch")
        {
            var url = configuration["Search:ElasticsearchUrl"] ?? "http://localhost:9200";
            services.AddSingleton(new ElasticsearchClient(new Uri(url)));
            services.AddScoped<ISearchService, ElasticsearchSearchService>();
            services.AddHostedService<ElasticsearchIndexInitializer>();
            healthChecksBuilder.AddElasticsearch(url, name: "elasticsearch", tags: ["search"]);
        }
        else
        {
            services.AddSingleton<ISearchService, InMemorySearchService>();
        }

        services.AddMassTransit(x =>
        {
            x.AddConsumer<MediaProcessingCompletedConsumer>();
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"), h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "mediaforge");
                    h.Password(configuration["RabbitMq:Password"] ?? "mediaforge_dev");
                });

                cfg.ReceiveEndpoint("media-processing-completed-search", e =>
                {
                    e.ConfigureConsumer<MediaProcessingCompletedConsumer>(ctx);
                    e.UseMessageRetry(r => r.Intervals(
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(30)));
                });
            });
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ISearchService).Assembly));

        return services;
    }
}
