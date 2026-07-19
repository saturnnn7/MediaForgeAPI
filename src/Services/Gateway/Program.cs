using System.Globalization;
using System.Threading.RateLimiting;
using MediaForge.Gateway.YARP.Consumers;
using MediaForge.Gateway.YARP.Hubs;
using MediaForge.Shared.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
       .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://localhost:5341")
       .Enrich.WithProperty("Service", "gateway")
       .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
});

// YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Auth - gateway validates tokens, forwards claims downstream
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Authority = builder.Configuration["IdentityServer:Authority"];
        opts.RequireHttpsMetadata = false;
        opts.MapInboundClaims = false; // keep raw claim names (e.g. "role") instead of the default ClaimTypes.* remapping
        opts.TokenValidationParameters = new()
        {
            ValidateAudience = false // downstream services validate their own audiences
        };
    });
builder.Services.AddAuthorization();

// Rate limiting (built-in .NET 8) - partitioned by user tier when authenticated, else by IP
builder.Services.AddRateLimiter(opts =>
{
    opts.AddPolicy("general", httpContext =>
    {
        var userId = httpContext.User?.FindFirst("sub")?.Value;
        var role = httpContext.User?.FindFirst("role")?.Value ?? "anonymous";

        int permitLimit = role switch
        {
            "admin" => 1000,
            "creator" => 200,
            "listener" => 60,
            _ => 30
        };

        var partitionKey = userId ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 5
            });
    });

    opts.AddPolicy("upload", httpContext =>
    {
        var role = httpContext.User?.FindFirst("role")?.Value ?? "anonymous";
        var userId = httpContext.User?.FindFirst("sub")?.Value;

        int permitLimit = role switch
        {
            "admin" => 100,
            "creator" => 20,
            _ => 1
        };

        var partitionKey = userId ?? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter($"upload:{partitionKey}", _ =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            });
    });

    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    opts.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"error\":\"Rate limit exceeded. Please slow down.\",\"retryAfter\":60}",
            cancellationToken);
    };
});

// SignalR with Redis backplane
var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConn, opts =>
    {
        opts.Configuration.ChannelPrefix = RedisChannel.Literal("mediaforge");
    });

// MassTransit - consumers only, no outbox (gateway doesn't publish, only consumes)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MediaProcessingCompletedConsumer>();
    x.AddConsumer<MediaProcessingFailedConsumer>();
    x.AddConsumer<NotifyUsersConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"), h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? "mediaforge");
            h.Password(builder.Configuration["RabbitMq:Password"] ?? "mediaforge_dev");
        });

        cfg.ReceiveEndpoint("gateway-media-completed", e =>
        {
            e.ConfigureConsumer<MediaProcessingCompletedConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("gateway-media-failed", e =>
        {
            e.ConfigureConsumer<MediaProcessingFailedConsumer>(ctx);
        });

        cfg.ReceiveEndpoint("gateway-notify-users", e =>
        {
            e.ConfigureConsumer<NotifyUsersConsumer>(ctx);
        });
    });
});

builder.Services.AddCors(opts =>
{
    opts.AddDefaultPolicy(policy =>
    {
        policy
            .SetIsOriginAllowed(_ => true)  // allow file:// and any origin in dev
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddHealthChecks()
    .AddRedis(redisConn, name: "redis", tags: ["cache"])
    .AddRabbitMqCheck(builder.Configuration.GetConnectionString("RabbitMq")!, tags: ["messaging"])
    .AddUrlGroup(new Uri(builder.Configuration["HealthChecks:IdentityApiUrl"]
                 ?? "http://localhost:5001/health/live"), name: "identity-api", tags: ["upstream"])
    .AddUrlGroup(new Uri(builder.Configuration["HealthChecks:MediaApiUrl"]
                 ?? "http://localhost:5002/health/live"), name: "media-api", tags: ["upstream"])
    .AddUrlGroup(new Uri(builder.Configuration["HealthChecks:CatalogApiUrl"]
                 ?? "http://localhost:5005/health/live"), name: "catalog-api", tags: ["upstream"]);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("gateway"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter(opts =>
            opts.Endpoint = new Uri(
                builder.Configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317")));

var app = builder.Build();
app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseRateLimiter();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// SignalR hub - before YARP so it doesn't get proxied
app.MapHub<NotificationHub>("/hubs/notifications")
   .RequireAuthorization();

app.MapReverseProxy();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            service = "gateway",
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
    Predicate = check => check.Tags.Contains("cache") || check.Tags.Contains("messaging")
});

app.Run();

public partial class Program;
