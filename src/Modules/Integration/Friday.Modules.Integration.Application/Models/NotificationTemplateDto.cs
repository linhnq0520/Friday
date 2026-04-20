namespace Friday.Modules.Integration.Application.Models;

public sealed record NotificationTemplateDto(
    int Id,
    string Channel,
    string TemplateCode,
    string Language,
    string SubjectTemplate,
    string BodyTemplate,
    bool IsActive,
    int Version,
    DateTime UpdatedOnUtc
);
