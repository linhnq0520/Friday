using Friday.Modules.Integration.Application.Models;
using Friday.Modules.Integration.Domain.Aggregates.NotificationTemplateAggregate;

namespace Friday.Modules.Integration.Application.Features.NotificationTemplates;

internal static class NotificationTemplateMapping
{
    public static NotificationTemplateDto ToDto(NotificationTemplate entity) =>
        new(
            entity.Id,
            entity.Channel,
            entity.TemplateCode,
            entity.Language,
            entity.SubjectTemplate,
            entity.BodyTemplate,
            entity.IsActive,
            entity.Version,
            entity.UpdatedOnUtc
        );
}
