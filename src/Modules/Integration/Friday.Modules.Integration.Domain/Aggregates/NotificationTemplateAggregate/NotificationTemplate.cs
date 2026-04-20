namespace Friday.Modules.Integration.Domain.Aggregates.NotificationTemplateAggregate;

public sealed class NotificationTemplate
{
    private NotificationTemplate() { }

    public int Id { get; private set; }
    public string Channel { get; private set; } = string.Empty;
    public string TemplateCode { get; private set; } = string.Empty;
    public string Language { get; private set; } = "en";
    public string SubjectTemplate { get; private set; } = string.Empty;
    public string BodyTemplate { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int Version { get; private set; }
    public DateTime UpdatedOnUtc { get; private set; }

    public static NotificationTemplate Create(
        string channel,
        string templateCode,
        string language,
        string subjectTemplate,
        string bodyTemplate,
        bool isActive,
        int version
    )
    {
        return new NotificationTemplate
        {
            Channel = channel.Trim().ToLowerInvariant(),
            TemplateCode = templateCode.Trim().ToUpperInvariant(),
            Language = language.Trim().ToLowerInvariant(),
            SubjectTemplate = subjectTemplate,
            BodyTemplate = bodyTemplate,
            IsActive = isActive,
            Version = version,
            UpdatedOnUtc = DateTime.UtcNow,
        };
    }

    public void UpdateContent(string subjectTemplate, string bodyTemplate, bool isActive)
    {
        SubjectTemplate = subjectTemplate;
        BodyTemplate = bodyTemplate;
        IsActive = isActive;
        UpdatedOnUtc = DateTime.UtcNow;
    }
}
