using MediaForge.Catalog.Infrastructure.Persistence;
using MediaForge.Catalog.Infrastructure.Persistence.Repositories;
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
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<ICatalogUnitOfWork>(sp => sp.GetRequiredService<CatalogDbContext>());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(IPersonRepository).Assembly,
            typeof(CatalogDbContext).Assembly));

        return services;
    }
}
