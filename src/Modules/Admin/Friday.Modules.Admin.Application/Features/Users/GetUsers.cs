using Friday.Modules.Admin.Application.Models;
using Friday.Modules.Admin.Domain.Repositories;
using LinKit.Core.Cqrs;

namespace Friday.Modules.Admin.Application.Features.Users;

public sealed record GetUsersQuery() : IQuery<IReadOnlyList<UserDto>>;

[CqrsHandler]
public sealed class GetUsersHandler(IUserRepository users)
    : IQueryHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> HandleAsync(
        GetUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Domain.Aggregates.UserAggregate.User> items = await users.ListAsync(
            cancellationToken
        );
        return items
            .Select(x =>
                new UserDto(
                    x.Id,
                    x.Username,
                    x.Email,
                    x.IsActive,
                    x.IsLocked,
                    x.UserRoles.Select(ur => ur.RoleId).ToArray()
                )
            )
            .ToArray();
    }
}
