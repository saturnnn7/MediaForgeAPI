using MediaForge.Media.API.DependencyInjection;
using MediaForge.Media.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediaServices(builder.Configuration);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapMediaEndpoints();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "media" }));
app.Run();

public partial class Program;
