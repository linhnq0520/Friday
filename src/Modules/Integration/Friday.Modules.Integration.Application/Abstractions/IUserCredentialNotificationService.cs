namespace Friday.Modules.Integration.Application.Abstractions;

public interface IUserCredentialNotificationService
{
    Task SendTemporaryPasswordAsync(
        string toEmail,
        string fullName,
        string temporaryPassword,
        CancellationToken cancellationToken = default
    );

    Task SendPasswordResetLinkAsync(
        string toEmail,
        string fullName,
        string resetLink,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default
    );
}
