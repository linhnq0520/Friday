using Friday.Modules.Admin.Application.Permissions;
using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using LinKit.Core.Cqrs;

namespace Friday.Modules.Admin.Application.Features.Rights;

/// <summary>
/// Inserts any <see cref="AdminPermissionCatalog"/> entries that are not yet persisted (idempotent).
/// </summary>
public sealed record SyncAdminPermissionCatalogCommand : ICommand<int>;

public sealed class SyncAdminPermissionCatalogHandler(IRightRepository rights)
    : ICommandHandler<SyncAdminPermissionCatalogCommand, int>
{
    public async Task<int> HandleAsync(
        SyncAdminPermissionCatalogCommand request,
        CancellationToken cancellationToken
    )
    {
        int added = 0;
        foreach (AdminPermissionCatalog.Definition def in AdminPermissionCatalog.All)
        {
            if (
                await rights.ExistsAsync(
                    def.Module,
                    def.Resource,
                    def.AccessLevel,
                    cancellationToken
                )
            )
            {
                continue;
            }

            Right right = Right.Create(
                def.Module,
                def.Resource,
                def.AccessLevel,
                def.Name,
                def.Description
            );
            await rights.AddAsync(right, cancellationToken);
            added++;
        }

        return added;
    }
}
