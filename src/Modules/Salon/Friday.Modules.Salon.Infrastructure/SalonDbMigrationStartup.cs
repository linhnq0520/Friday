using Friday.Modules.Salon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        if (!db.Database.IsRelational())
        {
            return;
        }

        await db.Database.MigrateAsync(cancellationToken);
    }
}
