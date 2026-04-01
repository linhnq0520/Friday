using Friday.BuildingBlocks.Application.Cqrs;
using LinKit.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Sample.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSampleApplication(this IServiceCollection services)
    {
        services.AddLinKitCqrs();
        services.AddCurrentLinKitMediatorAsKeyed(ModuleKeys.Sample);
        return services;
    }
}
