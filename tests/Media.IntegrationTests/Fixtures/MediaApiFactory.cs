using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediaForge.Media.Infrastructure.Persistence;
using MediaForge.Media.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace MediaForge.Media.IntegrationTests.Fixtures;

public sealed class MediaApiFactory(MediaInfrastructureFixture fixture) : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly SymmetricSecurityKey DummySigningKey =
        new(Encoding.UTF8.GetBytes("test-signing-key-not-validated-32b"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        var tempKeyPath = Path.Combine(Path.GetTempPath(), "mediaforge_test_media_public.pem");
        using (var rsa = RSA.Create(2048))
        {
            File.WriteAllText(tempKeyPath, rsa.ExportSubjectPublicKeyInfoPem());
        }

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MediaDb"] = fixture.PostgresConnectionString,
                ["ConnectionStrings:RabbitMq"] = fixture.RabbitMqConnectionString,
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["IdentityServer:Authority"] = "https://localhost:5001",
                ["IdentityServer:Audience"] = "media.read",
                ["Storage:ServiceUrl"] = fixture.MinioEndpoint,
                ["Storage:AccessKey"] = "minioadmin",
                ["Storage:SecretKey"] = "minioadmin",
                ["Grpc:IdentityServiceUrl"] = "https://localhost:5001",
                ["Jwt:PublicKeyPath"] = tempKeyPath
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace Redis with in-memory cache
            var redisDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDistributedCache)
                && d.ImplementationType?.Name.Contains("Redis", StringComparison.Ordinal) == true);
            if (redisDescriptor != null)
                services.Remove(redisDescriptor);

            services.AddDistributedMemoryCache();

            // Replace real gRPC Identity client with fake
            var grpcDescriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IIdentityGrpcClient));
            if (grpcDescriptor != null)
                services.Remove(grpcDescriptor);

            services.AddScoped<IIdentityGrpcClient, FakeIdentityGrpcClient>();

            // Remove MassTransit hosted services (bus not needed for these tests)
            // to avoid RabbitMQ connection noise in test output.
            var hostedServices = services
                .Where(d => d.ServiceType == typeof(IHostedService))
                .ToList();
            foreach (var hs in hostedServices)
                services.Remove(hs);

            // Accept any bearer token signature — the app validates against Identity's
            // real signing key in production, which isn't available here. A static
            // (empty) ConfigurationManager also stops JwtBearerHandler from trying to
            // fetch OIDC discovery metadata from the fake Authority URL.
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                opts.Configuration = new OpenIdConnectConfiguration();
                opts.ConfigurationManager = new StaticConfigurationManager<OpenIdConnectConfiguration>(opts.Configuration);

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
        var db = scope.ServiceProvider.GetRequiredService<MediaDbContext>();
        await db.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync() => await base.DisposeAsync();

    public HttpClient CreateAuthenticatedClient(Guid userId, string email)
    {
        var client = CreateClient();
        var token = GenerateUnsignedJwt(userId, email);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string GenerateUnsignedJwt(Guid userId, string email)
    {
        var signingCredentials = new SigningCredentials(DummySigningKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, email)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
