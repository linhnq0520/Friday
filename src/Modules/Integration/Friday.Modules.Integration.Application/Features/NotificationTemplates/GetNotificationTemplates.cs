using Friday.Modules.Integration.Application.Models;
using Friday.Modules.Integration.Domain.Repositories;
using LinKit.Core.Cqrs;

namespace Friday.Modules.Integration.Application.Features.NotificationTemplates;

public sealed record GetNotificationTemplatesQuery()
    : IQuery<IReadOnlyList<NotificationTemplateDto>>;

public sealed class GetNotificationTemplatesHandler(INotificationTemplateRepository repository)
    : IQueryHandler<GetNotificationTemplatesQuery, IReadOnlyList<NotificationTemplateDto>>
{
    public async Task<IReadOnlyList<NotificationTemplateDto>> HandleAsync(
        GetNotificationTemplatesQuery request,
        CancellationToken cancellationToken
    )
    {
        IReadOnlyList<Domain.Aggregates.NotificationTemplateAggregate.NotificationTemplate> entities =
            await repository.ListAsync(cancellationToken);
        return entities.Select(NotificationTemplateMapping.ToDto).ToList();
    }
}
