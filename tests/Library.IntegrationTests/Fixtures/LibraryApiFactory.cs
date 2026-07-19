using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediaForge.Library.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Library.IntegrationTests.Fixtures;

public sealed class LibraryApiFactory(LibraryInfrastructureFixture fixture) : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly SymmetricSecurityKey DummySigningKey =
        new(Encoding.UTF8.GetBytes("test-signing-key-not-validated-32b"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        var tempKeyPath = Path.Combine(Path.GetTempPath(), "mediaforge_test_library_public.pem");
        using (var rsa = RSA.Create(2048))
        {
            File.WriteAllText(tempKeyPath, rsa.ExportSubjectPublicKeyInfoPem());
        }

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:LibraryDb"] = fixture.PostgresConnectionString,
                ["ConnectionStrings:RabbitMq"] = fixture.RabbitMqConnectionString,
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["Jwt:PublicKeyPath"] = tempKeyPath
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

            // Remove MassTransit hosted services (bus not needed for these tests)
            // to avoid RabbitMQ connection noise in test output.
            var hostedServices = services
                .Where(d => d.ServiceType == typeof(IHostedService))
                .ToList();
            foreach (var hs in hostedServices)
                services.Remove(hs);

            // Accept any bearer token signature - Library validates against a real RSA
            // public key in production, which we don't have here.
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.TokenValidationParameters.SignatureValidator =
                    (token, _) => new JsonWebToken(token);
                opts.TokenValidationParameters.ValidateIssuer = false;
                opts.TokenValidationParameters.ValidateAudience = false;
                opts.TokenValidationParameters.ValidateLifetime = false;
            });
        });
    }

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
        await db.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync() => await base.DisposeAsync();

    public HttpClient CreateAuthenticatedClient(Guid userId, string role = "listener")
    {
        var client = CreateClient();
        var token = GenerateUnsignedJwt(userId, role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string GenerateUnsignedJwt(Guid userId, string role)
    {
        var signingCredentials = new SigningCredentials(DummySigningKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("role", role)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
