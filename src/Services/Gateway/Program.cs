using MediaForge.Gateway.YARP.Consumers;
using MediaForge.Gateway.YARP.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Auth — gateway validates tokens, forwards claims downstream
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.Authority = builder.Configuration["IdentityServer:Authority"];
        opts.RequireHttpsMetadata = false;
        opts.TokenValidationParameters = new()
        {
            ValidateAudience = false // downstream services validate their own audiences
        };
    });
builder.Services.AddAuthorization();

// Rate limiting (built-in .NET 8)
builder.Services.AddRateLimiter(opts =>
{
    opts.AddFixedWindowLimiter("general", o =>
    {
        o.PermitLimit = 100;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 10;
    });
    opts.AddFixedWindowLimiter("upload", o =>
    {
        o.PermitLimit = 10;
        o.Window = TimeSpan.FromMinutes(1);
        o.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 2;
    });
    opts.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// SignalR with Redis backplane
var redisConn = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConn, opts =>
    {
        opts.Configuration.ChannelPrefix = RedisChannel.Literal("mediaforge");
    });

// MassTransit — consumers only, no outbox (gateway doesn't publish, only consumes)
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MediaProcessingCompletedConsumer>();
    x.AddConsumer<MediaProcessingFailedConsumer>();
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
    });
});

builder.Services.AddSerilog(cfg => cfg.WriteTo.Console(
    formatProvider: System.Globalization.CultureInfo.InvariantCulture));

var app = builder.Build();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// SignalR hub — before YARP so it doesn't get proxied
app.MapHub<NotificationHub>("/hubs/notifications")
   .RequireAuthorization();

app.MapReverseProxy();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "gateway" }));

app.Run();
