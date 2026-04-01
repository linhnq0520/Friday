using System.Collections.Immutable;

namespace Friday.BuildingBlocks.Application.IntegrationEvents;

public sealed class IntegrationEventRouteRegistry : IIntegrationEventRouteRegistry
{
    private readonly Dictionary<Type, ImmutableArray<string>> _routes = new();

    public void Register<TEvent>(params string[] moduleKeys)
        where TEvent : IIntegrationEvent
    {
        ArgumentNullException.ThrowIfNull(moduleKeys);
        if (moduleKeys.Length == 0)
        {
            throw new ArgumentException("At least one module key is required.", nameof(moduleKeys));
        }

        string[] distinct = moduleKeys.Distinct(StringComparer.Ordinal).ToArray();
        _routes[typeof(TEvent)] = [.. distinct];
    }

    public IReadOnlyList<string> GetModuleKeys(Type eventType) =>
        _routes.TryGetValue(eventType, out ImmutableArray<string> keys) ? keys : [];
}
