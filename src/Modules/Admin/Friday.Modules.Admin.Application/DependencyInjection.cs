using Friday.BuildingBlocks.Application.Cqrs;
using LinKit.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Admin.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminApplication(this IServiceCollection services)
    {
        services.AddLinKitCqrs();
        services.AddCurrentLinKitMediatorAsKeyed(ModuleKeys.Admin);
        return services;
    }
}
