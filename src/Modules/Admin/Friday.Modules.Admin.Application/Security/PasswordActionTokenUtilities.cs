namespace Friday.Modules.Admin.Application.Security;

public static class PasswordActionTokenUtilities
{
    public static string GenerateKey() => RefreshTokenUtilities.GenerateOpaqueToken();

    public static string Hash(string key) => RefreshTokenUtilities.Hash(key);
}
