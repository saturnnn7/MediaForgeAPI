using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediaForge.Gateway.IntegrationTests.Fixtures;

public sealed class GatewayFactory(RabbitMqFixture fixture) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["ConnectionStrings:RabbitMq"] = fixture.ConnectionString,
                ["IdentityServer:Authority"] = "https://localhost:5001",
                ["RabbitMq:Username"] = "guest",
                ["RabbitMq:Password"] = "guest"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Program.cs wires SignalR to a Redis backplane, which isn't available in the test
            // environment. Remove the Redis HubLifetimeManager registration (AddSignalR's own
            // registration uses TryAdd, so it won't replace an existing one) to fall back to the
            // default in-process lifetime manager.
            var redisDescriptors = services
                .Where(d => d.ServiceType == typeof(HubLifetimeManager<>)
                    || (d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("Redis")))
                .ToList();
            foreach (var descriptor in redisDescriptors)
            {
                services.Remove(descriptor);
            }

            services.AddSignalR();

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.MapInboundClaims = false;
                opts.TokenValidationParameters.SignatureValidator =
                    (token, _) => new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token);
                opts.TokenValidationParameters.ValidateIssuer = false;
                opts.TokenValidationParameters.ValidateAudience = false;
                opts.TokenValidationParameters.ValidateLifetime = false;
                opts.RequireHttpsMetadata = false;
            });
        });
    }
}
