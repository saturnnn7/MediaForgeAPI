using System.Security.Cryptography;
using Duende.IdentityServer;
using FluentValidation;
using MassTransit;
using MediaForge.Identity.Application.Commands.Register;
using MediaForge.Identity.Infrastructure.Identity;
using MediaForge.Identity.Infrastructure.Persistence;
using MediaForge.Identity.Infrastructure.Persistence.Repositories;
using MediaForge.Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Identity.API.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly));
        services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);

        const string migrationsAssembly = "MediaForge.Identity.Infrastructure";

        services.AddDbContext<IdentityDbContext>(opts =>
            opts.UseNpgsql(
                configuration.GetConnectionString("IdentityDb"),
                sql => sql.MigrationsAssembly(migrationsAssembly)));

        services.AddIdentity<IdentityAppUser, IdentityRole<Guid>>(opts =>
            {
                opts.Password.RequiredLength = 8;
                opts.Password.RequireNonAlphanumeric = false;
                opts.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services.AddIdentityServer(opts =>
            {
                opts.IssuerUri = configuration["IdentityServer:IssuerUri"];
            })
            .AddConfigurationStore(opts => opts.ConfigureDbContext = b => b.UseNpgsql(
                configuration.GetConnectionString("IdentityDb"),
                sql => sql.MigrationsAssembly(migrationsAssembly)))
            .AddOperationalStore(opts =>
            {
                opts.ConfigureDbContext = b => b.UseNpgsql(
                    configuration.GetConnectionString("IdentityDb"),
                    sql => sql.MigrationsAssembly(migrationsAssembly));
                opts.EnableTokenCleanup = true;
            })
            .AddAspNetIdentity<IdentityAppUser>();

        services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                var privateKeyPath = configuration["Jwt:PrivateKeyPath"]!;
                var rsa = RSA.Create();
                rsa.ImportFromPem(File.ReadAllText(privateKeyPath));

                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
            });

        services.AddAuthorization();

        services.AddStackExchangeRedisCache(opts => opts.Configuration = configuration.GetConnectionString("Redis"));

        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IIdentityUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddSingleton<IAppSettings, AppSettings>();

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<IdentityDbContext>(o =>
            {
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"), h =>
                {
                    h.Username(configuration["RabbitMq:Username"] ?? "mediaforge");
                    h.Password(configuration["RabbitMq:Password"] ?? "mediaforge_dev");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });

        services.AddGrpc();

        return services;
    }
}
