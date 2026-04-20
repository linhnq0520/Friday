using Friday.Modules.Integration.Domain.Aggregates.NotificationTemplateAggregate;

namespace Friday.Modules.Integration.Domain.Repositories;

public interface INotificationTemplateRepository
{
    Task<IReadOnlyList<NotificationTemplate>> ListAsync(
        CancellationToken cancellationToken = default
    );
    Task<NotificationTemplate?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<NotificationTemplate> CreateAsync(
        string channel,
        string templateCode,
        string language,
        string subjectTemplate,
        string bodyTemplate,
        bool isActive,
        int? version,
        CancellationToken cancellationToken = default
    );
    Task<NotificationTemplate?> UpdateAsync(
        int id,
        string subjectTemplate,
        string bodyTemplate,
        bool isActive,
        CancellationToken cancellationToken = default
    );
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
