using FluentValidation;
using MediaForge.Identity.API.Grpc;
using MediaForge.Media.API.Grpc;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Media.API.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediaServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediaInfrastructure(configuration);

        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

        services.AddGrpcClient<UserService.UserServiceClient>(o =>
        {
            o.Address = new Uri(configuration["Grpc:IdentityServiceUrl"] ?? "https://localhost:5001");
        });
        services.AddScoped<IIdentityGrpcClient, IdentityGrpcClient>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                var publicKeyPath = configuration["Jwt:PublicKeyPath"]!;
                var publicKeyPem = File.ReadAllText(publicKeyPath);
                var rsa = RSA.Create();
                rsa.ImportFromPem(publicKeyPem);

                opts.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
                opts.RequireHttpsMetadata = false;
            });
        services.AddAuthorization();

        services.AddValidatorsFromAssembly(typeof(RequestUploadUrlCommand).Assembly);

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("MediaDb")!, name: "postgres", tags: ["db"])
            .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis", tags: ["cache"])
            .AddRabbitMQ(rabbitConnectionString: configuration.GetConnectionString("RabbitMq")!, name: "rabbitmq", tags: ["messaging"])
            .AddCheck("minio", () =>
            {
                var url = configuration["Storage:ServiceUrl"];
                return !string.IsNullOrEmpty(url)
                    ? HealthCheckResult.Healthy($"MinIO configured at {url}")
                    : HealthCheckResult.Unhealthy("MinIO not configured");
            }, tags: ["storage"]);

        return services;
    }
}
