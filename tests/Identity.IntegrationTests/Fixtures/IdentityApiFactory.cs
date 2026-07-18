using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using Duende.IdentityServer.EntityFramework.DbContexts;
using MediaForge.Identity.API.Data;
using MediaForge.Identity.Application.Abstractions;
using MediaForge.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Identity.IntegrationTests.Fixtures;

public sealed class IdentityApiFactory(DatabaseFixture fixture) : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly Lazy<(RSA Rsa, string PrivateKeyPath)> TestKey = new(GenerateTestKey);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:IdentityDb"] = fixture.PostgresConnectionString,
                ["ConnectionStrings:RabbitMq"] = fixture.RabbitMqConnectionString,
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["Jwt:PrivateKeyPath"] = TestKey.Value.PrivateKeyPath,
                ["Jwt:AccessTokenExpiryMinutes"] = "15",
                ["Jwt:RefreshTokenExpiryDays"] = "7",
                ["IdentityServer:IssuerUri"] = "https://localhost",
                ["Email:Host"] = "localhost",
                ["Email:Port"] = "25",
                ["Email:Username"] = "",
                ["Email:Password"] = "",
                ["Email:From"] = "test@test.local"
            });
        });

        builder.ConfigureServices(services =>
        {
            var redisDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDistributedCache)
                && d.ImplementationType?.Name.Contains("Redis", StringComparison.Ordinal) == true);
            if (redisDescriptor != null)
                services.Remove(redisDescriptor);

            services.AddDistributedMemoryCache();

            // Real SMTP isn't available in the test environment - swap in a no-op fake.
            var emailDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));
            if (emailDescriptor != null)
                services.Remove(emailDescriptor);

            services.AddScoped<IEmailService, NoOpEmailService>();

            // Duende's TokenCleanupHost has a flaky start/stop lifecycle under WebApplicationFactory's
            // fast test host - it's a periodic maintenance task with no bearing on these tests.
            var tokenCleanupDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IHostedService)
                && d.ImplementationType?.Name == "TokenCleanupHost");
            if (tokenCleanupDescriptor != null)
                services.Remove(tokenCleanupDescriptor);
        });
    }

    private static (RSA Rsa, string PrivateKeyPath) GenerateTestKey()
    {
        var rsa = RSA.Create(2048);
        var path = Path.Combine(Path.GetTempPath(), "mediaforge_test_private.pem");
        File.WriteAllText(path, rsa.ExportRSAPrivateKeyPem());
        return (rsa, path);
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();

        var identityDb = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await identityDb.Database.MigrateAsync();

        var configDb = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
        await configDb.Database.MigrateAsync();

        var persistedGrantDb = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
        await persistedGrantDb.Database.MigrateAsync();

        await IdentityServerSeeder.SeedAsync(Services);
    }

    async Task IAsyncLifetime.DisposeAsync() => await base.DisposeAsync();

    public HttpClient CreateAuthenticatedClient(string userId, string email)
    {
        var client = CreateClient();
        var token = GenerateTestJwt(userId, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string GenerateTestJwt(string userId, string email)
    {
        var (rsa, _) = TestKey.Value;
        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
