using MediaForge.Identity.API.Data;
using MediaForge.Identity.API.DependencyInjection;
using MediaForge.Identity.API.Endpoints;
using MediaForge.Identity.API.Grpc;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddIdentityServices(builder.Configuration);

// builder.WebHost.ConfigureKestrel(opts =>
// {
//     opts.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http1AndHttp2);
// });

var app = builder.Build();
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

app.Run();

public partial class Program;
