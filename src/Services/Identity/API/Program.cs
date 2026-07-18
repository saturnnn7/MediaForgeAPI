using System.Globalization;
using MediaForge.Identity.API.Data;
using MediaForge.Identity.API.DependencyInjection;
using MediaForge.Identity.API.Endpoints;
using MediaForge.Identity.API.Grpc;
using MediaForge.Identity.Infrastructure.Persistence;
using MediaForge.Shared.Infrastructure.Migrations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
       .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://localhost:5341")
       .Enrich.WithProperty("Service", "identity")
       .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
});

builder.Services.AddIdentityServices(builder.Configuration);

// builder.WebHost.ConfigureKestrel(opts =>
// {
//     opts.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http1AndHttp2);
// });

var app = builder.Build();
app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});
app.UseRouting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<UserGrpcService>();
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapChannelEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            service = "identity",
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db") || check.Tags.Contains("cache")
});

if (app.Environment.IsDevelopment())
{
    await IdentityServerSeeder.SeedAsync(app.Services);
}

if (app.Environment.IsProduction() || app.Environment.IsEnvironment("Docker"))
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    await MigrationRunner.RunMigrationsAsync<IdentityDbContext>(app.Services, logger);
    await MigrationRunner.RunMigrationsAsync<ConfigurationDbContext>(app.Services, logger);
    await MigrationRunner.RunMigrationsAsync<PersistedGrantDbContext>(app.Services, logger);
    await IdentityServerSeeder.SeedAsync(app.Services);
}

app.Run();

public partial class Program;
