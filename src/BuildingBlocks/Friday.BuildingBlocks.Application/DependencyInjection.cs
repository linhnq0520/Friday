using Friday.BuildingBlocks.Application.IntegrationEvents;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocksApplication(this IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// Registers integration event routing and publisher. Call after module CQRS is registered so keyed mediators exist.
    /// </summary>
    public static IServiceCollection AddIntegrationEventsRouting(
        this IServiceCollection services,
        Action<IIntegrationEventRouteRegistry> configure
    )
    {
        ArgumentNullException.ThrowIfNull(configure);

        var registry = new IntegrationEventRouteRegistry();
        configure(registry);
        services.AddSingleton<IIntegrationEventRouteRegistry>(registry);
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
        return services;
    }
}
