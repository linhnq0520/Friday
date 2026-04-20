using Friday.API.Common;
using Friday.Modules.Admin.Application.Authorization;
using Friday.Modules.Admin.Application.Permissions;
using Friday.Modules.Integration.Application.Features.NotificationTemplates;
using LinKit.Core.Cqrs;

namespace Friday.API.Modules.Integration;

public static class IntegrationEndpoints
{
    public sealed record CreateNotificationTemplateRequest(
        string Channel,
        string TemplateCode,
        string Language,
        string SubjectTemplate,
        string BodyTemplate,
        bool IsActive,
        int? Version
    );

    public sealed record UpdateNotificationTemplateRequest(
        string SubjectTemplate,
        string BodyTemplate,
        bool IsActive
    );

    public static IEndpointRouteBuilder MapIntegrationModule(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints
            .MapGroup("/api/integration")
            .WithTags("Integration")
            .RequireAuthorization();

        group
            .MapGet(
                "/notification-templates",
                async (
                    HttpContext context,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var response = await mediator.QueryAsync(
                        new GetNotificationTemplatesQuery(),
                        cancellationToken
                    );
                    return ApiResults.Ok(context, response);
                }
            )
            .RequireAuthorization(
                AdminPermissionEndpointPolicy.ForPermission(
                    AdminPermissionKeys.NotificationTemplatesRead
                )
            );

        group
            .MapGet(
                "/notification-templates/{id:int}",
                async (
                    HttpContext context,
                    int id,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var response = await mediator.QueryAsync(
                        new GetNotificationTemplateByIdQuery(id),
                        cancellationToken
                    );
                    return ApiResults.Ok(context, response);
                }
            )
            .RequireAuthorization(
                AdminPermissionEndpointPolicy.ForPermission(
                    AdminPermissionKeys.NotificationTemplatesRead
                )
            );

        group
            .MapPost(
                "/notification-templates",
                async (
                    HttpContext context,
                    CreateNotificationTemplateRequest body,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var response = await mediator.SendAsync(
                        new CreateNotificationTemplateCommand(
                            body.Channel,
                            body.TemplateCode,
                            body.Language,
                            body.SubjectTemplate,
                            body.BodyTemplate,
                            body.IsActive,
                            body.Version
                        ),
                        cancellationToken
                    );
                    return ApiResults.Ok(context, response);
                }
            )
            .RequireAuthorization(
                AdminPermissionEndpointPolicy.ForPermission(
                    AdminPermissionKeys.NotificationTemplatesManage
                )
            );

        group
            .MapPut(
                "/notification-templates/{id:int}",
                async (
                    HttpContext context,
                    int id,
                    UpdateNotificationTemplateRequest body,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var response = await mediator.SendAsync(
                        new UpdateNotificationTemplateCommand(
                            id,
                            body.SubjectTemplate,
                            body.BodyTemplate,
                            body.IsActive
                        ),
                        cancellationToken
                    );
                    return ApiResults.Ok(context, response);
                }
            )
            .RequireAuthorization(
                AdminPermissionEndpointPolicy.ForPermission(
                    AdminPermissionKeys.NotificationTemplatesManage
                )
            );

        group
            .MapDelete(
                "/notification-templates/{id:int}",
                async (
                    HttpContext context,
                    int id,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    bool deleted = await mediator.SendAsync(
                        new DeleteNotificationTemplateCommand(id),
                        cancellationToken
                    );
                    return ApiResults.Ok(context, new { deleted });
                }
            )
            .RequireAuthorization(
                AdminPermissionEndpointPolicy.ForPermission(
                    AdminPermissionKeys.NotificationTemplatesManage
                )
            );

        return endpoints;
    }
}
