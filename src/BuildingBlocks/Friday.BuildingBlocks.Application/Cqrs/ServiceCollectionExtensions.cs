using LinKit.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Application.Cqrs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCurrentLinKitMediatorAsKeyed(
        this IServiceCollection services,
        string key
    )
    {
        ServiceDescriptor mediatorDescriptor =
            services.LastOrDefault(x => x.ServiceType == typeof(IMediator))
            ?? throw new InvalidOperationException(
                "IMediator is not registered. Call AddLinKitCqrs() first."
            );

        services.AddKeyedScoped<IMediator>(
            key,
            (serviceProvider, _) =>
            {
                if (mediatorDescriptor.ImplementationInstance is IMediator instance)
                {
                    return instance;
                }

                if (mediatorDescriptor.ImplementationFactory is not null)
                {
                    return (IMediator)mediatorDescriptor.ImplementationFactory(serviceProvider);
                }

                if (mediatorDescriptor.ImplementationType is not null)
                {
                    return (IMediator)
                        ActivatorUtilities.CreateInstance(
                            serviceProvider,
                            mediatorDescriptor.ImplementationType
                        );
                }

                throw new InvalidOperationException("Unsupported IMediator registration shape.");
            }
        );

        return services;
    }
}
