using FluentMigrator;
using Friday.BuildingBlocks.Infrastructure.Hosting;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Admin.Domain.Aggregates.RoleAggregate;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Friday.Modules.Admin.Domain.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Infrastructure.DataMigrations;

[FridayDataMigration(
    "2026-04-18T13:00:00Z",
    "Data: admin.users/user_roles — bootstrap administrator account with ADMIN role."
)]
public sealed class Data_20260418130000_Admin_BootstrapAdministrator : AutoReversingMigration
{
    private const string AdminRoleCode = "ADMIN";
    private const string AdminUserCode = "ADMIN001";
    private const string AdminUsername = "admin";
    private const string AdminEmail = "admin@friday.local";
    private const string AdminFullName = "System Administrator";
    private const string AdminPassword = "Admin@123456";

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
        IPasswordHasher<CredentialUser> passwordHasher = sp.GetRequiredService<
            IPasswordHasher<CredentialUser>
        >();

        Role? adminRole = await db.Set<Role>()
            .FirstOrDefaultAsync(x => x.Code == AdminRoleCode, cancellationToken);
        if (adminRole is null)
        {
            throw new InvalidOperationException(
                $"Required role '{AdminRoleCode}' not found. Ensure default role migration ran first."
            );
        }

        User? admin = await db.Set<User>()
            .Include(x => x.UserRoles)
            .Include(x => x.PasswordCredential)
            .FirstOrDefaultAsync(
                x =>
                    x.Username == AdminUsername
                    || x.Email == AdminEmail
                    || x.UserCode == AdminUserCode,
                cancellationToken
            );

        string passwordHash = passwordHasher.HashPassword(new CredentialUser(), AdminPassword);

        if (admin is null)
        {
            admin = User.Create(
                AdminUserCode,
                AdminUsername,
                AdminEmail,
                AdminFullName,
                null,
                null,
                null,
                null,
                null
            );
            admin.SetPasswordCredential(UserPassword.Create(admin, passwordHash));
            admin.AssignRole(adminRole.Id);
            db.Add(admin);
            await db.SaveChangesAsync(cancellationToken);
            return;
        }

        admin.SetOrUpdatePasswordHash(passwordHash);
        admin.AssignRole(adminRole.Id);
        await db.SaveChangesAsync(cancellationToken);
    }
}
