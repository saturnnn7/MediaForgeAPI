using Amazon.S3;
using MassTransit;
using MediaForge.Media.Infrastructure.Consumers;
using MediaForge.Media.Infrastructure.Persistence;
using MediaForge.Media.Infrastructure.Persistence.Repositories;
using MediaForge.Media.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaForge.Media.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediaInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        const string migrationsAssembly = "MediaForge.Media.Infrastructure";

        services.AddDbContext<MediaDbContext>(opts =>
            opts.UseNpgsql(
                configuration.GetConnectionString("MediaDb"),
                sql => sql.MigrationsAssembly(migrationsAssembly)));

        services.AddScoped<IMediaAssetRepository, MediaAssetRepository>();
        services.AddScoped<IMediaUnitOfWork>(sp => sp.GetRequiredService<MediaDbContext>());
        services.AddScoped<IUserContextService, UserContextService>();

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var serviceUrl = configuration["Storage:ServiceUrl"] ?? "http://localhost:9000";
            var accessKey = configuration["Storage:AccessKey"] ?? "mediaforge";
            var secretKey = configuration["Storage:SecretKey"] ?? "mediaforge_dev_s3cret";

            var s3Config = new AmazonS3Config
            {
                ServiceURL = serviceUrl,
                ForcePathStyle = true,
                UseHttp = true,
                HttpClientFactory = null,
                Timeout = TimeSpan.FromSeconds(10),
                MaxErrorRetry = 1
            };

            return new AmazonS3Client(
                new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey),
                s3Config
            );
        });

        services.AddScoped<IStorageService, StorageService>();
        services.AddScoped<ISilenceDetectionService, SilenceDetectionService>();
        services.AddScoped<IMediaProcessingClient, SilenceDetectionClient>();
        services.AddHttpContextAccessor();

        services.AddHostedService<MinioInitializer>();

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<MediaDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<MediaProcessingCompletedConsumer>();
            x.AddConsumer<MediaProcessingFailedConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"), h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "mediaforge");
                    h.Password(configuration["RabbitMq:Password"] ?? "mediaforge_dev");
                });

                cfg.ReceiveEndpoint("media-processing-completed-media", e =>
                {
                    e.ConfigureConsumer<MediaProcessingCompletedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint("media-processing-failed-media", e =>
                {
                    e.ConfigureConsumer<MediaProcessingFailedConsumer>(ctx);
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
            typeof(IMediaAssetRepository).Assembly,
            typeof(MediaDbContext).Assembly));

        return services;
    }
}
