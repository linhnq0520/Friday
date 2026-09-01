using Friday.Modules.Salon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Friday.Modules.Salon.Infrastructure;

public static class SalonDbMigrationStartup
{
    public static async Task ApplyMigrationsAsync(
        IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default
    )
    {
        if (
            !configuration.GetValue(
                "Database:ApplyMigrationsOnStartup",
                false
            )
        )
        {
            return;
        }

        await using AsyncServiceScope scope = services.CreateAsyncScope();
        SalonDbContext db = scope.ServiceProvider.GetRequiredService<SalonDbContext>();
        ILogger? logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(typeof(SalonDbMigrationStartup));

        if (!db.Database.IsRelational())
        {
            return;
        }

        try
        {
            // Clear any stale migration lock left by a previous crashed or aborted migration run
            try
            {
                await db.Database.ExecuteSqlRawAsync("DELETE FROM \"__EFMigrationsLock\";", cancellationToken);
            }
            catch
            {
                // Table __EFMigrationsLock might not exist yet on a fresh database
            }

            IEnumerable<string> pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pending.Any())
            {
                logger?.LogInformation(
                    "Applying {Count} pending Salon migrations: {Migrations}",
                    pending.Count(),
                    string.Join(", ", pending)
                );
                await db.Database.MigrateAsync(cancellationToken);
                logger?.LogInformation("Salon database migrations applied successfully.");
            }
            else
            {
                logger?.LogInformation("Salon database is already up-to-date with migrations.");
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred while applying Salon database migrations.");
            throw;
        }
    }
}
