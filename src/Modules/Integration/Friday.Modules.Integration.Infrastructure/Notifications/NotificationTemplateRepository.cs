using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Integration.Domain.Aggregates.NotificationTemplateAggregate;
using Friday.Modules.Integration.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Friday.Modules.Integration.Infrastructure.Notifications;

public sealed class NotificationTemplateRepository(FridayDbContext dbContext)
    : INotificationTemplateRepository
{
    public async Task<IReadOnlyList<NotificationTemplate>> ListAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Set<NotificationTemplate>()
            .AsNoTracking()
            .OrderBy(x => x.Channel)
            .ThenBy(x => x.TemplateCode)
            .ThenBy(x => x.Language)
            .ThenByDescending(x => x.Version)
            .ToListAsync(cancellationToken);
    }

    public async Task<NotificationTemplate?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Set<NotificationTemplate>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<NotificationTemplate> CreateAsync(
        string channel,
        string templateCode,
        string language,
        string subjectTemplate,
        string bodyTemplate,
        bool isActive,
        int? version,
        CancellationToken cancellationToken = default
    )
    {
        string normalizedChannel = channel.Trim().ToLowerInvariant();
        string normalizedCode = templateCode.Trim().ToUpperInvariant();
        string normalizedLanguage = language.Trim().ToLowerInvariant();

        int nextVersion =
            version
            ?? (
                await dbContext
                    .Set<NotificationTemplate>()
                    .Where(x =>
                        x.Channel == normalizedChannel
                        && x.TemplateCode == normalizedCode
                        && x.Language == normalizedLanguage
                    )
                    .Select(x => (int?)x.Version)
                    .MaxAsync(cancellationToken) ?? 0
            ) + 1;

        if (isActive)
        {
            await SetActiveAsync(
                normalizedChannel,
                normalizedCode,
                normalizedLanguage,
                null,
                cancellationToken
            );
        }

        NotificationTemplate entity = NotificationTemplate.Create(
            normalizedChannel,
            normalizedCode,
            normalizedLanguage,
            subjectTemplate,
            bodyTemplate,
            isActive,
            nextVersion
        );
        await dbContext.Set<NotificationTemplate>().AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<NotificationTemplate?> UpdateAsync(
        int id,
        string subjectTemplate,
        string bodyTemplate,
        bool isActive,
        CancellationToken cancellationToken = default
    )
    {
        NotificationTemplate? entity = await dbContext
            .Set<NotificationTemplate>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (isActive)
        {
            await SetActiveAsync(
                entity.Channel,
                entity.TemplateCode,
                entity.Language,
                id,
                cancellationToken
            );
        }

        entity.UpdateContent(subjectTemplate, bodyTemplate, isActive);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        NotificationTemplate? entity = await dbContext
            .Set<NotificationTemplate>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return false;
        }

        dbContext.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task SetActiveAsync(
        string channel,
        string templateCode,
        string language,
        int? keepId,
        CancellationToken cancellationToken
    )
    {
        List<NotificationTemplate> existing = await dbContext
            .Set<NotificationTemplate>()
            .Where(x =>
                x.Channel == channel
                && x.TemplateCode == templateCode
                && x.Language == language
                && x.IsActive
                && (!keepId.HasValue || x.Id != keepId.Value)
            )
            .ToListAsync(cancellationToken);

        foreach (NotificationTemplate item in existing)
        {
            item.UpdateContent(item.SubjectTemplate, item.BodyTemplate, false);
        }
    }
}
