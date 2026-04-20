using Friday.Modules.Admin.Application.Configuration;
using Friday.Modules.Admin.Application.Security;
using Friday.Modules.Integration.Application.Abstractions;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using LinKit.Core.Cqrs;
using Microsoft.Extensions.Options;

namespace Friday.Modules.Admin.Application.Features.Auth;

public sealed record RequestPasswordResetCommand(string Email) : ICommand<bool>;

public sealed class RequestPasswordResetCommandHandler(
    IUserRepository users,
    IUserPasswordActionRepository passwordActions,
    IUserCredentialNotificationService notifications,
    IOptions<PasswordFlowOptions> passwordFlowOptions
) : ICommandHandler<RequestPasswordResetCommand, bool>
{
    public async Task<bool> HandleAsync(
        RequestPasswordResetCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return true;
        }

        User? user = await users.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return true;
        }

        await passwordActions.InvalidateActiveForUserAsync(
            user.Id,
            PasswordActionTypes.Reset,
            cancellationToken
        );

        string key = PasswordActionTokenUtilities.GenerateKey();
        string hash = PasswordActionTokenUtilities.Hash(key);
        int ttlMinutes = Math.Clamp(passwordFlowOptions.Value.ForgotPasswordTokenMinutes, 5, 24 * 60);
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(ttlMinutes);
        await passwordActions.AddAsync(
            UserPasswordAction.Create(user.Id, PasswordActionTypes.Reset, hash, expiresAt),
            cancellationToken
        );

        string resetLink = BuildResetLink(passwordFlowOptions.Value, key);
        await notifications.SendPasswordResetLinkAsync(
            user.Email,
            user.FullName,
            resetLink,
            expiresAt,
            cancellationToken
        );

        return true;
    }

    private static string BuildResetLink(PasswordFlowOptions options, string key)
    {
        if (string.IsNullOrWhiteSpace(options.AppBaseUrl))
        {
            return $"<configure Authentication:PasswordFlow:AppBaseUrl>/auth/reset-password?key={Uri.EscapeDataString(key)}";
        }

        string baseUrl = options.AppBaseUrl.TrimEnd('/');
        string path = string.IsNullOrWhiteSpace(options.ResetPath) ? "/auth/reset-password" : options.ResetPath;
        string normalizedPath = path.StartsWith('/') ? path : "/" + path;
        return $"{baseUrl}{normalizedPath}?key={Uri.EscapeDataString(key)}";
    }
}
