using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Friday.BuildingBlocks.Infrastructure.Persistence;

public static class DatabaseMigrationStartup
{
    /// <summary>
    /// Applies EF Core schema migrations, then FluentMigrator data migrations. Skips when disabled, when the DbContext is not relational, or when FluentMigrator was not registered (in-memory database).
    /// </summary>
    public static async Task ApplyEfThenDataMigrationsAsync(
        this IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default
    )
    {
        if (
            !configuration.GetValue(
                $"{DatabaseOptions.SectionName}:{nameof(DatabaseOptions.ApplyMigrationsOnStartup)}",
                false
            )
        )
        {
            return;
        }

        await using AsyncServiceScope scope = services.CreateAsyncScope();
        FridayDbContext db = scope.ServiceProvider.GetRequiredService<FridayDbContext>();
        ILogger? logger = scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger(typeof(DatabaseMigrationStartup));

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
                    "Applying {Count} pending EF Core migrations: {Migrations}",
                    pending.Count(),
                    string.Join(", ", pending)
                );
                await db.Database.MigrateAsync(cancellationToken);
                logger?.LogInformation("EF Core migrations applied successfully.");
            }
            else
            {
                logger?.LogInformation("EF Core database is already up-to-date with migrations.");
            }

            IMigrationRunner? runner = scope.ServiceProvider.GetService<IMigrationRunner>();
            runner?.MigrateUp();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred while applying database migrations.");
            throw;
        }
    }
}
