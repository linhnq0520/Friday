using LinKit.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Application.IntegrationEvents;

public sealed class IntegrationEventPublisher(
    IServiceProvider serviceProvider,
    IIntegrationEventRouteRegistry registry
) : IIntegrationEventPublisher
{
    public async Task PublishAsync<TEvent>(
        TEvent integrationEvent,
        CancellationToken cancellationToken = default
    )
        where TEvent : IIntegrationEvent
    {
        IReadOnlyList<string> keys = registry.GetModuleKeys(typeof(TEvent));
        foreach (string key in keys)
        {
            IMediator mediator = serviceProvider.GetRequiredKeyedService<IMediator>(key);
            await mediator.PublishAsync(
                integrationEvent,
                PublishStrategy.Sequential,
                cancellationToken
            );
        }
    }
}
