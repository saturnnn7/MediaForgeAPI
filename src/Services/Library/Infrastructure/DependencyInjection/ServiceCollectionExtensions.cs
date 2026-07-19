using MassTransit;
using MediaForge.Library.Infrastructure.Consumers;
using MediaForge.Library.Infrastructure.Persistence;
using MediaForge.Library.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaForge.Library.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLibraryInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        const string migrationsAssembly = "MediaForge.Library.Infrastructure";

        services.AddDbContext<LibraryDbContext>(opts =>
            opts.UseNpgsql(
                configuration.GetConnectionString("LibraryDb"),
                sql => sql.MigrationsAssembly(migrationsAssembly)));

        services.AddScoped<ILibraryEntryRepository, LibraryEntryRepository>();
        services.AddScoped<IListeningProgressRepository, ListeningProgressRepository>();
        services.AddScoped<IUserListRepository, UserListRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<ILibraryUnitOfWork>(sp => sp.GetRequiredService<LibraryDbContext>());

        services.AddStackExchangeRedisCache(opts =>
            opts.Configuration = configuration.GetConnectionString("Redis"));

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<LibraryDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<PartPublishedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"), h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "mediaforge");
                    h.Password(configuration["RabbitMq:Password"] ?? "mediaforge_dev");
                });

                cfg.ReceiveEndpoint("library-part-published", e =>
                {
                    e.ConfigureConsumer<PartPublishedConsumer>(ctx);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(ILibraryEntryRepository).Assembly,
            typeof(LibraryDbContext).Assembly));

        services.AddHttpContextAccessor();

        return services;
    }
}
