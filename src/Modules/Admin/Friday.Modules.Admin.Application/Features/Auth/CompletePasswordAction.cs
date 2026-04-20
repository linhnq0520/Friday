using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using Friday.Modules.Admin.Application.Security;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using Friday.Modules.Admin.Domain.Security;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Friday.Modules.Admin.Application.Features.Auth;

public sealed record CompletePasswordActionCommand(string Key, string NewPassword) : ICommand<bool>;

public sealed class CompletePasswordActionCommandHandler(
    IUserRepository users,
    IUserSessionRepository sessions,
    IUserPasswordActionRepository passwordActions,
    IPasswordHasher<CredentialUser> passwordHasher
) : ICommandHandler<CompletePasswordActionCommand, bool>
{
    private static readonly CredentialUser CredentialMarker = new();

    public async Task<bool> HandleAsync(
        CompletePasswordActionCommand request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(request.Key))
        {
            throw new FridayException(
                ErrorCodes.Admin.PasswordActionKeyInvalid,
                "Password action key is required.",
                StatusCodes.Status400BadRequest
            );
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword))
        {
            throw new FridayException(ErrorCodes.Admin.PasswordRequired, "Password is required.");
        }

        string keyHash = PasswordActionTokenUtilities.Hash(request.Key);
        UserPasswordAction? action = await passwordActions.GetActiveByTokenHashAsync(
            keyHash,
            cancellationToken
        );
        if (action is null)
        {
            throw new FridayException(
                ErrorCodes.Admin.PasswordActionExpired,
                "Password action is invalid or expired.",
                StatusCodes.Status400BadRequest
            );
        }

        User? user = await users.GetByIdWithPasswordAsync(action.UserId, cancellationToken);
        if (user is null)
        {
            throw new FridayException(
                ErrorCodes.Admin.UserNotFound,
                "User is no longer available.",
                StatusCodes.Status404NotFound
            );
        }

        string hash = passwordHasher.HashPassword(CredentialMarker, request.NewPassword);
        user.SetOrUpdatePasswordHash(hash);
        user.ClearPasswordChangeRequired();
        action.Consume();

        await sessions.RevokeAllForUserAsync(user.Id, cancellationToken);
        return true;
    }
}
