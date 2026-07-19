using System.Security.Cryptography;
using FluentValidation;
using MediaForge.Catalog.API.Services;
using MediaForge.Catalog.Application.Abstractions;
using MediaForge.Catalog.Application.Commands.CreatePerson;
using MediaForge.Catalog.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Catalog.API.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCatalogInfrastructure(configuration);

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
                opts.MapInboundClaims = false;
            });
        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddValidatorsFromAssembly(typeof(CreatePersonCommand).Assembly);

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("CatalogDb")!, name: "postgres", tags: ["db"]);

        return services;
    }
}
