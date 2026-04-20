using System.Net;
using System.Net.Mail;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Integration.Application.Abstractions;
using Friday.Modules.Integration.Application.Configuration;
using Friday.Modules.Integration.Domain.Aggregates.NotificationTemplateAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Friday.Modules.Integration.Infrastructure.Notifications;

public sealed class UserCredentialNotificationService(
    FridayDbContext dbContext,
    IOptions<SmtpOptions> smtpOptions,
    ILogger<UserCredentialNotificationService> logger
) : IUserCredentialNotificationService
{
    private const string Channel = "email";
    private const string TempPasswordTemplateCode = "USER_TEMP_PASSWORD";
    private const string PasswordResetTemplateCode = "PASSWORD_RESET";
    private const string DefaultLanguage = "en";

    public Task SendTemporaryPasswordAsync(
        string toEmail,
        string fullName,
        string temporaryPassword,
        CancellationToken cancellationToken = default
    )
    {
        return SendWithTemplateAsync(
            toEmail,
            TempPasswordTemplateCode,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["FullName"] = fullName,
                ["TemporaryPassword"] = temporaryPassword,
            },
            "Friday account credentials",
            "Hello {{FullName}},\n\nYour account has been created by an administrator.\nTemporary password: {{TemporaryPassword}}\nYou must change this password after your first sign-in.\n",
            cancellationToken
        );
    }

    public Task SendPasswordResetLinkAsync(
        string toEmail,
        string fullName,
        string resetLink,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default
    )
    {
        return SendWithTemplateAsync(
            toEmail,
            PasswordResetTemplateCode,
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["FullName"] = fullName,
                ["ResetLink"] = resetLink,
                ["ExpiresAtUtc"] = expiresAtUtc.ToString("O"),
            },
            "Friday password reset",
            "Hello {{FullName}},\n\nWe received a request to reset your password.\nReset link: {{ResetLink}}\nThis link expires at {{ExpiresAtUtc}}.\n",
            cancellationToken
        );
    }

    private async Task SendWithTemplateAsync(
        string toEmail,
        string templateCode,
        IReadOnlyDictionary<string, string> model,
        string fallbackSubject,
        string fallbackBody,
        CancellationToken cancellationToken
    )
    {
        (string subjectTemplate, string bodyTemplate) = await ResolveTemplateAsync(
            templateCode,
            cancellationToken
        );

        string subject = RenderTemplate(
            string.IsNullOrWhiteSpace(subjectTemplate) ? fallbackSubject : subjectTemplate,
            model
        );
        string body = RenderTemplate(
            string.IsNullOrWhiteSpace(bodyTemplate) ? fallbackBody : bodyTemplate,
            model
        );

        await SendEmailAsync(toEmail, subject, body, cancellationToken);
    }

    private async Task<(string SubjectTemplate, string BodyTemplate)> ResolveTemplateAsync(
        string templateCode,
        CancellationToken cancellationToken
    )
    {
        NotificationTemplate? template = await dbContext
            .Set<NotificationTemplate>()
            .AsNoTracking()
            .Where(x =>
                x.Channel == Channel
                && x.TemplateCode == templateCode
                && x.Language == DefaultLanguage
                && x.IsActive
            )
            .OrderByDescending(x => x.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (template is null)
        {
            return (string.Empty, string.Empty);
        }

        return (template.SubjectTemplate, template.BodyTemplate);
    }

    private static string RenderTemplate(string template, IReadOnlyDictionary<string, string> model)
    {
        string output = template;
        foreach ((string key, string value) in model)
        {
            output = output.Replace(
                "{{" + key + "}}",
                value ?? string.Empty,
                StringComparison.Ordinal
            );
        }

        return output;
    }

    private async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken
    )
    {
        SmtpOptions options = smtpOptions.Value;
        if (!options.Enabled || string.IsNullOrWhiteSpace(options.Host))
        {
            logger.LogWarning(
                "SMTP disabled. Intended email to {To}: {Subject}. Body: {Body}",
                toEmail,
                subject,
                body
            );
            return;
        }

        using MailMessage message = new();
        message.From = new MailAddress(options.FromAddress, options.FromName);
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.Body = body;

        using SmtpClient client = new(options.Host, options.Port) { EnableSsl = options.EnableSsl };

        if (!string.IsNullOrWhiteSpace(options.Username))
        {
            client.Credentials = new NetworkCredential(options.Username, options.Password);
        }

        cancellationToken.ThrowIfCancellationRequested();
        await client.SendMailAsync(message);
    }
}
