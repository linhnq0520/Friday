using Friday.API.Common;
using Friday.BuildingBlocks.Application.Cqrs;
using Friday.Modules.Admin.Application.Features.Rights;
using Friday.Modules.Admin.Application.Features.Roles;
using Friday.Modules.Admin.Application.Features.Users;
using LinKit.Core.Cqrs;

namespace Friday.API.Modules.Admin;

public static class AdminEndpoints
{
    public static IEndpointRouteBuilder MapAdminModule(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/admin").WithTags("Admin");

        group.MapPost(
            "/users",
            async (
                HttpContext context,
                CreateUserCommand command,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.SendAsync(command, cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        group.MapGet(
            "/users",
            async (
                HttpContext context,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.QueryAsync(new GetUsersQuery(), cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        group.MapPost(
            "/users/{userId:int}/roles/{roleId:int}",
            async (
                HttpContext context,
                int userId,
                int roleId,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.SendAsync(
                    new AssignRoleToUserCommand(userId, roleId),
                    cancellationToken
                );
                return ApiResults.Ok(context, response);
            }
        );

        group.MapPost(
            "/users/{userId:int}/lock",
            async (
                HttpContext context,
                int userId,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.SendAsync(
                    new LockUserCommand(userId),
                    cancellationToken
                );
                return ApiResults.Ok(context, response);
            }
        );

        group.MapPost(
            "/roles",
            async (
                HttpContext context,
                CreateRoleCommand command,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.SendAsync(command, cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        group.MapGet(
            "/roles",
            async (
                HttpContext context,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.QueryAsync(new GetRolesQuery(), cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        group.MapPost(
            "/roles/{roleId:int}/rights",
            async (
                HttpContext context,
                int roleId,
                int[] rightIds,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.SendAsync(
                    new GrantRightsToRoleCommand(roleId, rightIds),
                    cancellationToken
                );
                return ApiResults.Ok(context, response);
            }
        );

        group.MapPost(
            "/rights",
            async (
                HttpContext context,
                CreateRightCommand command,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.SendAsync(command, cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        group.MapGet(
            "/rights",
            async (
                HttpContext context,
                [FromKeyedServices(ModuleKeys.Admin)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.QueryAsync(new GetRightsQuery(), cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        return endpoints;
    }
}
