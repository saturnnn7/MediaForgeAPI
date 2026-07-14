using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
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
            var redisDescriptor = services.SingleOrDefault(d =>
                d.ServiceType.FullName != null &&
                d.ServiceType.FullName.Contains("Redis"));
            if (redisDescriptor != null)
            {
                services.Remove(redisDescriptor);
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
