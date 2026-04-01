namespace Friday.BuildingBlocks.Application.IntegrationEvents;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TEvent>(
        TEvent integrationEvent,
        CancellationToken cancellationToken = default
    )
        where TEvent : IIntegrationEvent;
}
