namespace Friday.BuildingBlocks.Application.IntegrationEvents;

public interface IIntegrationEventRouteRegistry
{
    void Register<TEvent>(params string[] moduleKeys)
        where TEvent : IIntegrationEvent;

    IReadOnlyList<string> GetModuleKeys(Type eventType);
}
