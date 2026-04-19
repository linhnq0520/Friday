using Friday.Modules.Admin.Application.Models;
using Friday.Modules.Admin.Domain.Aggregates.RightAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using LinKit.Core.Cqrs;

namespace Friday.Modules.Admin.Application.Features.Rights;

public sealed record GetRightsQuery() : IQuery<IReadOnlyList<RightDto>>;

public sealed class GetRightsHandler(IRightRepository rights)
    : IQueryHandler<GetRightsQuery, IReadOnlyList<RightDto>>
{
    public async Task<IReadOnlyList<RightDto>> HandleAsync(
        GetRightsQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Right> items = await rights.ListAsync(cancellationToken);
        return items.Select(ToDto).ToArray();
    }

    private static RightDto ToDto(Right x)
    {
        string level = x.AccessLevel == PermissionAccessLevel.Manage ? "manage" : "read";
        return new RightDto(x.Id, x.Module, x.Resource, level, x.Name, x.Description, x.PermissionKey);
    }
}
