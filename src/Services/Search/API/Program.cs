using FluentValidation;
using MediaForge.Search.API.Endpoints;
using MediaForge.Search.Application.Queries.SearchMedia;
using MediaForge.Search.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSearchInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Authority = builder.Configuration["IdentityServer:Authority"];
        opts.Audience = builder.Configuration["IdentityServer:Audience"];
        opts.RequireHttpsMetadata = false;
    });
builder.Services.AddAuthorization();
builder.Services.AddValidatorsFromAssembly(typeof(SearchMediaQuery).Assembly);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapSearchEndpoints();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            service = "search",
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
    Predicate = check => check.Tags.Contains("search") || check.Tags.Contains("messaging")
});

app.Run();
