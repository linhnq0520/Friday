using Friday.BuildingBlocks.Application.Cqrs;
using Friday.API.Common;
using Friday.Modules.Sample.Application.Features;
using LinKit.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.API.Modules.Sample;

public static class SampleEndpoints
{
    public static IEndpointRouteBuilder MapSampleModule(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/sample").WithTags("Sample");

        group.MapPost(
            "/todos",
            async (
                HttpContext context,
                CreateTodoItemCommand command,
                [FromKeyedServices(ModuleKeys.Sample)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.SendAsync(command, cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        group.MapGet(
            "/todos",
            async (
                HttpContext context,
                [FromKeyedServices(ModuleKeys.Sample)] IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var response = await mediator.QueryAsync(new GetTodoItemsQuery(), cancellationToken);
                return ApiResults.Ok(context, response);
            }
        );

        return endpoints;
    }
}
