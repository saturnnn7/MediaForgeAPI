using MassTransit;
using MediaForge.Catalog.Infrastructure.Consumers;
using MediaForge.Catalog.Infrastructure.Persistence;
using MediaForge.Catalog.Infrastructure.Persistence.Repositories;
using MediaForge.Catalog.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaForge.Catalog.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        const string migrationsAssembly = "MediaForge.Catalog.Infrastructure";

        services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseNpgsql(
                configuration.GetConnectionString("CatalogDb"),
                sql => sql.MigrationsAssembly(migrationsAssembly)));

        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<ISeriesRepository, SeriesRepository>();
        services.AddScoped<IWorkRepository, WorkRepository>();
        services.AddScoped<IEditionRepository, EditionRepository>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<ICatalogUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddStackExchangeRedisCache(opts =>
            opts.Configuration = configuration.GetConnectionString("Redis"));
        services.AddScoped<ISubscriptionService, SubscriptionCacheService>();

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<UserSubscribedConsumer>();
            x.AddConsumer<UserUnsubscribedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"), h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "mediaforge");
                    h.Password(configuration["RabbitMq:Password"] ?? "mediaforge_dev");
                });

                cfg.ReceiveEndpoint("catalog-user-subscribed", e =>
                {
                    e.ConfigureConsumer<UserSubscribedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint("catalog-user-unsubscribed", e =>
                {
                    e.ConfigureConsumer<UserUnsubscribedConsumer>(ctx);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(IPersonRepository).Assembly,
            typeof(CatalogDbContext).Assembly));

        return services;
    }
}
