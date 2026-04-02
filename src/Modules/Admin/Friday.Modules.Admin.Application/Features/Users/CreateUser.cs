using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using Friday.Modules.Admin.Application.Models;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using LinKit.Core.Cqrs;
using LinKit.Core.Endpoints;

namespace Friday.Modules.Admin.Application.Features.Users;

public sealed record CreateUserCommand(string Username, string Email, int[] RoleIds)
    : ICommand<UserDto>;

public sealed class CreateUserHandler(IUserRepository users, IRoleRepository roles)
    : ICommandHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> HandleAsync(
        CreateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        if (await users.ExistsByUsernameAsync(request.Username, cancellationToken))
        {
            throw new FridayException(
                ErrorCodes.Admin.UserUsernameExists,
                "Username already exists."
            );
        }

        if (await users.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new FridayException(ErrorCodes.Admin.UserEmailExists, "Email already exists.");
        }

        int[] roleIds = request.RoleIds.Distinct().ToArray();
        if (roleIds.Length > 0)
        {
            IReadOnlyList<Domain.Aggregates.RoleAggregate.Role> existingRoles =
                await roles.GetByIdsAsync(roleIds, cancellationToken);
            if (existingRoles.Count != roleIds.Length)
            {
                throw new FridayException(
                    ErrorCodes.Admin.UserRoleNotFound,
                    "Some roles are not found."
                );
            }
        }

        User user = User.Create(request.Username, request.Email);
        foreach (int roleId in roleIds)
        {
            user.AssignRole(roleId);
        }

        await users.AddAsync(user, cancellationToken);

        return new UserDto(
            user.Id,
            user.Username,
            user.Email,
            user.IsActive,
            user.IsLocked,
            user.UserRoles.Select(x => x.RoleId).ToArray()
        );
    }
}
