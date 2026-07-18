using System.Globalization;
using Amazon.Runtime;
using Amazon.S3;
using MediaForge.Processing.Worker.Consumers;
using MediaForge.Processing.Worker.Services;
using MediaForge.Shared.Infrastructure.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenAI;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(
    new BasicAWSCredentials(
        builder.Configuration["Storage:AccessKey"],
        builder.Configuration["Storage:SecretKey"]),
    new AmazonS3Config
    {
        ServiceURL = builder.Configuration["Storage:ServiceUrl"],
        ForcePathStyle = true,
        UseHttp = true,
        Timeout = TimeSpan.FromSeconds(30),
        MaxErrorRetry = 1
    }
));

builder.Services.AddSingleton(_ =>
{
    var key = builder.Configuration["OpenAI:ApiKey"];
    return new OpenAIClient(string.IsNullOrEmpty(key) ? "dummy-key-not-used" : key);
});

builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IMediaProcessingService, MediaProcessingService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MediaUploadedConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"), h =>
        {
            h.Username(builder.Configuration["RabbitMq:Username"] ?? "mediaforge");
            h.Password(builder.Configuration["RabbitMq:Password"] ?? "mediaforge_dev");
        });

        cfg.ReceiveEndpoint("media-uploaded", e =>
        {
            e.ConfigureConsumer<MediaUploadedConsumer>(ctx);
            e.UseMessageRetry(r => r.Intervals(
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(30),
                TimeSpan.FromMinutes(2)));
        });
    });
});

FFMpegCore.GlobalFFOptions.Configure(opts =>
    opts.BinaryFolder = builder.Configuration["FFmpeg:BinaryPath"] ?? string.Empty);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
       .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://localhost:5341")
       .Enrich.WithProperty("Service", "processing-worker")
       .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
});

builder.Services.AddHealthChecks()
    .AddRabbitMqCheck(builder.Configuration.GetConnectionString("RabbitMq")!, tags: ["messaging"]);

builder.WebHost.UseUrls("http://localhost:5004");

var app = builder.Build();
app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate =
        "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            service = "processing-worker",
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

await app.RunAsync();
