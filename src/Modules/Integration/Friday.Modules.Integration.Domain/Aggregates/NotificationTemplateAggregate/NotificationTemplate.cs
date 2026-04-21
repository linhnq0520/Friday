using Friday.BuildingBlocks.Domain.Entities;

namespace Friday.Modules.Integration.Domain.Aggregates.NotificationTemplateAggregate;

public sealed class NotificationTemplate : AggregateRoot
{
    private NotificationTemplate() { }

    public string Channel { get; private set; } = string.Empty;
    public string TemplateCode { get; private set; } = string.Empty;
    public string Language { get; private set; } = "en";
    public string SubjectTemplate { get; private set; } = string.Empty;
    public string BodyTemplate { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int Version { get; private set; }

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
        if (version <= 0)
        {
            throw new ArgumentException("Version must be greater than zero.", nameof(version));
        }

        return new NotificationTemplate
        {
            Channel = NormalizeChannel(channel),
            TemplateCode = NormalizeTemplateCode(templateCode),
            Language = NormalizeLanguage(language),
            SubjectTemplate = NormalizeRequiredText(subjectTemplate, nameof(subjectTemplate)),
            BodyTemplate = NormalizeRequiredText(bodyTemplate, nameof(bodyTemplate)),
            IsActive = isActive,
            Version = version,
        };
    }

    public void UpdateContent(string subjectTemplate, string bodyTemplate, bool isActive)
    {
        SubjectTemplate = NormalizeRequiredText(subjectTemplate, nameof(subjectTemplate));
        BodyTemplate = NormalizeRequiredText(bodyTemplate, nameof(bodyTemplate));
        IsActive = isActive;
        Touch();
    }

    private static string NormalizeChannel(string channel) =>
        NormalizeRequiredText(channel, nameof(channel)).ToLowerInvariant();

    private static string NormalizeTemplateCode(string templateCode) =>
        NormalizeRequiredText(templateCode, nameof(templateCode)).ToUpperInvariant();

    private static string NormalizeLanguage(string language) =>
        NormalizeRequiredText(language, nameof(language)).ToLowerInvariant();

    private static string NormalizeRequiredText(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} is required.", paramName);
        }

        return value.Trim();
    }
}
