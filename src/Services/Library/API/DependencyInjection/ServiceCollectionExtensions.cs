using System.Security.Cryptography;
using FluentValidation;
using MediaForge.Library.API.Services;
using MediaForge.Library.Application.Abstractions;
using MediaForge.Library.Application.Commands.AddToLibrary;
using MediaForge.Library.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Library.API.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLibraryServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLibraryInfrastructure(configuration);

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

        services.AddValidatorsFromAssembly(typeof(AddToLibraryCommand).Assembly);

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("LibraryDb")!, name: "postgres", tags: ["db"]);

        return services;
    }
}
