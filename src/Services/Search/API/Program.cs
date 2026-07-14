using FluentValidation;
using MediaForge.Search.API.Endpoints;
using MediaForge.Search.Application.Queries.SearchMedia;
using MediaForge.Search.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "search" }));
app.Run();
