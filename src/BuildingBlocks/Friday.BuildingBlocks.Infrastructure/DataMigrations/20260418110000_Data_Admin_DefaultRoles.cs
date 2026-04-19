using FluentMigrator;
using Friday.BuildingBlocks.Infrastructure.Hosting;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Admin.Domain.Aggregates.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Infrastructure.DataMigrations;

[FridayDataMigration(
    "2026-04-18T11:00:00Z",
    "Data: admin.roles — default system roles (ADMIN, SUPPORT, VIEWER)."
)]
public sealed class Data_20260418110000_Admin_DefaultRoles : AutoReversingMigration
{
    public override void Up()
    {
        ApplicationServiceProviderAccessor
            .ExecuteInNewScopeAsync(ApplySeedAsync, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    private static async Task ApplySeedAsync(
        IServiceProvider sp,
        CancellationToken cancellationToken
    )
    {
        FridayDbContext db = sp.GetRequiredService<FridayDbContext>();

        foreach (AdminRoleRightDefaultsSeed.RoleRow row in AdminRoleRightDefaultsSeed.Roles)
        {
            Role? existing = await db.Set<Role>()
                .FirstOrDefaultAsync(x => x.Code == row.Code, cancellationToken);
            if (existing is null)
            {
                db.Add(Role.Create(row.Code, row.Name));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
