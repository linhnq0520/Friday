using Microsoft.AspNetCore.Identity;

namespace Friday.Modules.Salon.Application.Security;

public sealed class AdminPasswordService : IAdminPasswordService
{
    private readonly PasswordHasher<AdminCredentialUser> _hasher = new();

    public string HashPassword(string password) =>
        _hasher.HashPassword(new AdminCredentialUser(), password);

    public bool VerifyPassword(string password, string passwordHash)
    {
        PasswordVerificationResult result = _hasher.VerifyHashedPassword(
            new AdminCredentialUser(),
            passwordHash,
            password
        );
        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }

    private sealed class AdminCredentialUser;
}
