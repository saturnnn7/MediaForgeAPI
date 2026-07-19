using System.Security.Cryptography;
using Duende.IdentityServer;
using FluentValidation;
using MassTransit;
using MediaForge.Identity.Application.Commands.Register;
using MediaForge.Identity.Infrastructure.Identity;
using MediaForge.Identity.Infrastructure.Persistence;
using MediaForge.Identity.Infrastructure.Persistence.Repositories;
using MediaForge.Identity.Infrastructure.Services;
using MediaForge.Shared.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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

        services.Configure<CookieAuthenticationOptions>(
            IdentityConstants.ExternalScheme,
            opts =>
            {
                opts.Cookie.SameSite = SameSiteMode.Lax;
                opts.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

        services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.MapInboundClaims = false;

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
            })
            .AddGoogle(opts =>
            {
                opts.ClientId = configuration["Authentication:Google:ClientId"] ?? string.Empty;
                opts.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
                opts.CallbackPath = "/signin-google";
                opts.SignInScheme = IdentityConstants.ExternalScheme;

                opts.Scope.Add("email");
                opts.Scope.Add("profile");

                opts.ClaimActions.MapJsonKey("picture", "picture");
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("CreatorOnly", p => p.RequireClaim("role", "creator"))
            .AddPolicy("AdminOnly", p => p.RequireClaim("role", "admin"))
            .AddPolicy("OptionalAuth", p => p
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAssertion(_ => true));

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("IdentityDb")!,
                       name: "postgres", tags: ["db"])
            .AddRedis(configuration.GetConnectionString("Redis")!,
                      name: "redis", tags: ["cache"])
            .AddRabbitMqCheck(configuration.GetConnectionString("RabbitMq")!, tags: ["messaging"]);

        services.AddStackExchangeRedisCache(opts => opts.Configuration = configuration.GetConnectionString("Redis"));

        services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.AddScoped<IAuthorFollowRepository, AuthorFollowRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IIdentityUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IAppSettings, AppSettings>();
        services.AddHttpContextAccessor();

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

        services.AddOpenTelemetry()
            .WithTracing(tracing => tracing
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService("identity"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation()
                .AddOtlpExporter(opts =>
                    opts.Endpoint = new Uri(
                        configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317")));

        return services;
    }
}
