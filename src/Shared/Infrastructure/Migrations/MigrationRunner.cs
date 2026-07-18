using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MediaForge.Shared.Infrastructure.Migrations;

public static class MigrationRunner
{
    public static async Task RunMigrationsAsync<TContext>(
        IServiceProvider services,
        ILogger logger) where TContext : DbContext
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();
        var pendingMigrations = await db.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            logger.LogInformation("Applying {Count} pending migrations for {Context}...",
                pendingMigrations.Count(), typeof(TContext).Name);
            await db.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully for {Context}.",
                typeof(TContext).Name);
        }
    }
}
