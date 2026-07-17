using Microsoft.Extensions.Configuration;

namespace MediaForge.Identity.Infrastructure.Services;

public sealed class AppSettings(IConfiguration configuration) : IAppSettings
{
    public string BaseUrl => configuration["App:BaseUrl"] ?? "http://localhost:5001";
}
