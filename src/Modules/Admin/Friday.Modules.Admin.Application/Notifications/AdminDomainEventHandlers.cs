using Friday.BuildingBlocks.Application.IntegrationEvents;
using Friday.Modules.Admin.Domain.Events;
using LinKit.Core.Cqrs;
using Microsoft.Extensions.Logging;

namespace Friday.Modules.Admin.Application.Notifications;

[CqrsHandler]
public sealed class UserCreatedDomainEventHandler(
    ILogger<UserCreatedDomainEventHandler> logger,
    IIntegrationEventPublisher integrationEvents
) : INotificationHandler<UserCreatedDomainEvent>
{
    public async Task HandleAsync(
        UserCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Admin domain event: user created. UserId={UserId}, Username={Username}, Email={Email}",
            notification.UserId,
            notification.Username,
            notification.Email
        );
        await integrationEvents.PublishAsync(
            new UserCreatedIntegrationEvent(
                notification.UserId,
                notification.Username,
                notification.Email
            ),
            cancellationToken
        );
    }
}

[CqrsHandler]
public sealed class UserRoleAssignedDomainEventHandler(
    ILogger<UserRoleAssignedDomainEventHandler> logger
) : INotificationHandler<UserRoleAssignedDomainEvent>
{
    public Task HandleAsync(
        UserRoleAssignedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Admin domain event: role assigned. UserId={UserId}, RoleId={RoleId}",
            notification.UserId,
            notification.RoleId
        );
        return Task.CompletedTask;
    }
}

[CqrsHandler]
public sealed class RoleRightsChangedDomainEventHandler(
    ILogger<RoleRightsChangedDomainEventHandler> logger
) : INotificationHandler<RoleRightsChangedDomainEvent>
{
    public Task HandleAsync(
        RoleRightsChangedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation(
            "Admin domain event: role rights changed. RoleId={RoleId}, RightCount={RightCount}",
            notification.RoleId,
            notification.RightIds.Length
        );
        return Task.CompletedTask;
    }
}
