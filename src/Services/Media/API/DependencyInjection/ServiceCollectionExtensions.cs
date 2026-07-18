using FluentValidation;
using MediaForge.Identity.API.Grpc;
using MediaForge.Media.API.Grpc;
using MediaForge.Media.Application.Abstractions;
using MediaForge.Media.Infrastructure.DependencyInjection;
using MediaForge.Shared.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
        })
        .AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromMilliseconds(200);

            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.MinimumThroughput = 5;
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);

            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
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
            .AddRabbitMqCheck(configuration.GetConnectionString("RabbitMq")!, tags: ["messaging"])
            .AddCheck("minio", () =>
            {
                var url = configuration["Storage:ServiceUrl"];
                return !string.IsNullOrEmpty(url)
                    ? HealthCheckResult.Healthy($"MinIO configured at {url}")
                    : HealthCheckResult.Unhealthy("MinIO not configured");
            }, tags: ["storage"]);

        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("media"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(opts =>
                    opts.Endpoint = new Uri(
                        configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317")));

        return services;
    }
}
