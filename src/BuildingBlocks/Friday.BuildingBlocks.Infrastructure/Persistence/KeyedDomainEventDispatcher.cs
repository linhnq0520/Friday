using Friday.BuildingBlocks.Application.Abstractions;
using Friday.BuildingBlocks.Domain.Abstractions;
using Friday.BuildingBlocks.Domain.Entities;
using LinKit.Core.Cqrs;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Infrastructure.Persistence;

public sealed class KeyedDomainEventDispatcher(IServiceProvider serviceProvider)
    : IDomainEventDispatcher
{
    public async Task DispatchAsync(
        IReadOnlyCollection<Entity> entities,
        CancellationToken cancellationToken = default
    )
    {
        foreach (Entity entity in entities)
        {
            if (entity.DomainEvents.Count == 0)
            {
                continue;
            }

            string? moduleKey = ResolveModuleKey(entity.GetType().Namespace);
            if (string.IsNullOrWhiteSpace(moduleKey))
            {
                entity.ClearDomainEvents();
                continue;
            }

            IMediator mediator = serviceProvider.GetRequiredKeyedService<IMediator>(moduleKey);
            foreach (IDomainEvent domainEvent in entity.DomainEvents)
            {
                await mediator.PublishAsync(
                    domainEvent,
                    PublishStrategy.Sequential,
                    cancellationToken
                );
            }

            entity.ClearDomainEvents();
        }
    }

    private static string? ResolveModuleKey(string? entityNamespace)
    {
        if (string.IsNullOrWhiteSpace(entityNamespace))
        {
            return null;
        }

        const string modulesPrefix = "Friday.Modules.";
        int modulesIndex = entityNamespace.IndexOf(modulesPrefix, StringComparison.Ordinal);
        if (modulesIndex < 0)
        {
            return null;
        }

        string remainder = entityNamespace[(modulesIndex + modulesPrefix.Length)..];
        int split = remainder.IndexOf('.');
        string moduleName = split > 0 ? remainder[..split] : remainder;
        return moduleName.ToLowerInvariant();
    }
}
