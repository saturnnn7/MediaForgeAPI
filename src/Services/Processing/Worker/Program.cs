using System.Globalization;
using Amazon.Runtime;
using Amazon.S3;
using MediaForge.Processing.Worker.Consumers;
using MediaForge.Processing.Worker.Services;
using OpenAI;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

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

builder.Logging.ClearProviders();
builder.Services.AddSerilog(cfg => cfg.WriteTo.Console(formatProvider: CultureInfo.InvariantCulture));

var host = builder.Build();
await host.RunAsync();
