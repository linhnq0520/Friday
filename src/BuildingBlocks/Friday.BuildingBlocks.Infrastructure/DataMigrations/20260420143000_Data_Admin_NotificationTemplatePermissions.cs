using FluentMigrator;
using Friday.BuildingBlocks.Infrastructure.Hosting;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Admin.Application.Permissions;
using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;
using Friday.Modules.Admin.Domain.Aggregates.RoleAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Infrastructure.DataMigrations;

[FridayDataMigration(
    "2026-04-20T14:30:00Z",
    "Data: admin.rights/admin.role_rights — add notification template permissions."
)]
public sealed class Data_20260420143000_Admin_NotificationTemplatePermissions : AutoReversingMigration
{
    public override void Up()
    {
        ApplicationServiceProviderAccessor
            .ExecuteInNewScopeAsync(ApplySeedAsync, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }

    private static async Task ApplySeedAsync(IServiceProvider sp, CancellationToken cancellationToken)
    {
        FridayDbContext db = sp.GetRequiredService<FridayDbContext>();

        foreach (AdminPermissionCatalog.Definition def in AdminPermissionCatalog.All)
        {
            string m = def.Module.Trim().ToLowerInvariant();
            string r = def.Resource.Trim().ToLowerInvariant();
            bool exists = await db.Set<Right>()
                .AnyAsync(
                    x => x.Module == m && x.Resource == r && x.AccessLevel == def.AccessLevel,
                    cancellationToken
                );
            if (!exists)
            {
                db.Add(
                    Right.Create(
                        def.Module,
                        def.Resource,
                        def.AccessLevel,
                        def.Name,
                        def.Description
                    )
                );
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        Dictionary<(string Module, string Resource, PermissionAccessLevel Level), int> rightByKey =
            await db.Set<Right>()
                .ToDictionaryAsync(
                    x => (x.Module, x.Resource, x.AccessLevel),
                    x => x.Id,
                    cancellationToken
                );
        Dictionary<string, int> roleByCode = await db.Set<Role>()
            .ToDictionaryAsync(x => x.Code, x => x.Id, cancellationToken);

        foreach (
            AdminRoleRightDefaultsSeed.RolePermissionsRow assignment in AdminRoleRightDefaultsSeed.RolePermissions
        )
        {
            if (!roleByCode.TryGetValue(assignment.RoleCode, out int roleId))
            {
                continue;
            }

            int[] rightIds = assignment
                .Permissions.Select(p =>
                {
                    string mod = p.Module.Trim().ToLowerInvariant();
                    string res = p.Resource.Trim().ToLowerInvariant();
                    return rightByKey.TryGetValue((mod, res, p.AccessLevel), out int id)
                        ? id
                        : (int?)null;
                })
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToArray();

            await db.Set<RoleRight>()
                .Where(x => x.RoleId == roleId)
                .ExecuteDeleteAsync(cancellationToken);

            foreach (int rightId in rightIds)
            {
                db.Add(RoleRight.Create(roleId, rightId));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
