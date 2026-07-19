using System.Globalization;
using MediaForge.Catalog.API.DependencyInjection;
using MediaForge.Catalog.API.Endpoints;
using MediaForge.Catalog.Infrastructure.Persistence;
using MediaForge.Shared.Infrastructure.Migrations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
       .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://localhost:5341")
       .Enrich.WithProperty("Service", "catalog")
       .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
});

builder.Services.AddCatalogServices(builder.Configuration);

var app = builder.Build();

app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapPersonEndpoints();
app.MapGenreEndpoints();
app.MapSeriesEndpoints();
app.MapWorkEndpoints();
app.MapEditionEndpoints();
app.MapPartEndpoints();
app.MapChannelEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            service = "catalog",
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

if (app.Environment.IsProduction() || app.Environment.IsEnvironment("Docker"))
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    await MigrationRunner.RunMigrationsAsync<CatalogDbContext>(app.Services, logger);
}

app.Run();

public partial class Program;
