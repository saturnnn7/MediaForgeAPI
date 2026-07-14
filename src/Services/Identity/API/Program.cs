using MediaForge.Identity.API.Data;
using MediaForge.Identity.API.DependencyInjection;
using MediaForge.Identity.API.Endpoints;
using MediaForge.Identity.API.Grpc;
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
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "identity" }));

if (app.Environment.IsDevelopment())
{
    await IdentityServerSeeder.SeedAsync(app.Services);
}

app.Run();

public partial class Program;
