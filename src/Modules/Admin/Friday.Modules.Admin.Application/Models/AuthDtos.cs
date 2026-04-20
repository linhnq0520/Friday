namespace Friday.Modules.Admin.Application.Models;

public sealed record LoginResponseDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    UserDto User,
    bool RequirePasswordChange = false,
    string? PasswordActionKey = null
);

public sealed record RefreshTokenResponseDto(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken
);
