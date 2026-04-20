using System.Text;
using Friday.BuildingBlocks.Application.Abstractions;
using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using Friday.Modules.Admin.Application.Configuration;
using Friday.Modules.Admin.Application.Models;
using Friday.Modules.Admin.Application.Security;
using Friday.Modules.Admin.Domain.Aggregates.UserAggregate;
using Friday.Modules.Admin.Domain.Repositories;
using Friday.Modules.Admin.Domain.Security;
using LinKit.Core.Cqrs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Friday.Modules.Admin.Application.Features.Auth;

public sealed record OAuth2LoginCommand(string Provider, string Code, string RedirectUri)
    : ICommand<LoginResponseDto>;

public sealed class OAuth2LoginCommandHandler(
    IUserRepository users,
    IUserSessionRepository sessions,
    IRoleRepository roles,
    IJwtTokenIssuer jwt,
    IOAuth2ProfileClient oAuth2ProfileClient,
    IUnitOfWork unitOfWork,
    IOptions<JwtSettings> jwtSettings,
    IOptionsMonitor<RegistrationOptions> registrationOptions,
    IHttpContextAccessor httpContextAccessor
) : ICommandHandler<OAuth2LoginCommand, LoginResponseDto>
{
    public async Task<LoginResponseDto> HandleAsync(
        OAuth2LoginCommand request,
        CancellationToken cancellationToken
    )
    {
        OAuth2UserProfile profile = await oAuth2ProfileClient.ExchangeCodeForProfileAsync(
            request.Provider,
            request.Code,
            request.RedirectUri,
            cancellationToken
        );

        User? user = await users.GetByEmailAsync(profile.Email, cancellationToken);
        if (user is null)
        {
            if (!registrationOptions.CurrentValue.AllowPublicRegistration)
            {
                throw new FridayException(
                    ErrorCodes.Admin.RegistrationDisabled,
                    "Public registration is disabled.",
                    StatusCodes.Status403Forbidden
                );
            }

            user = await CreateOAuth2UserAsync(profile, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }

        if (!user.IsActive)
        {
            throw new FridayException(
                ErrorCodes.Admin.UserInactive,
                "User account is inactive.",
                StatusCodes.Status403Forbidden
            );
        }

        if (user.IsLocked)
        {
            throw new FridayException(
                ErrorCodes.Admin.UserLockedAuth,
                "User account is locked.",
                StatusCodes.Status403Forbidden
            );
        }

        int[] roleIds = user.UserRoles.Select(x => x.RoleId).ToArray();
        IReadOnlyList<Domain.Aggregates.RoleAggregate.Role> roleEntities =
            await roles.GetByIdsAsync(roleIds, cancellationToken);
        string[] roleCodes = roleEntities.Where(x => x.IsActive).Select(x => x.Code).Distinct().ToArray();

        string refreshToken = RefreshTokenUtilities.GenerateOpaqueToken();
        string refreshHash = RefreshTokenUtilities.Hash(refreshToken);
        int refreshDays = Math.Clamp(jwtSettings.Value.RefreshTokenDays, 1, 365);
        DateTime refreshExpires = DateTime.UtcNow.AddDays(refreshDays);

        HttpContext? http = httpContextAccessor.HttpContext;
        string? ip = http?.Connection.RemoteIpAddress?.ToString();
        string? ua = http?.Request.Headers.UserAgent.ToString();

        UserSession session = UserSession.Create(
            user.Id,
            refreshHash,
            refreshExpires,
            string.IsNullOrWhiteSpace(ip) ? null : ip,
            string.IsNullOrWhiteSpace(ua) ? null : ua
        );

        await sessions.AddAsync(session, cancellationToken);

        JwtAccessTokenResult access = jwt.CreateAccessToken(user.Id, session.Id, roleCodes);
        return new LoginResponseDto(access.Token, access.ExpiresAtUtc, refreshToken, UserDto.FromUser(user));
    }

    private async Task<User> CreateOAuth2UserAsync(
        OAuth2UserProfile profile,
        CancellationToken cancellationToken
    )
    {
        string userCode = await GenerateUniqueUserCodeAsync(cancellationToken);
        string username = await GenerateUniqueUsernameAsync(profile.Email, cancellationToken);
        string fullName = string.IsNullOrWhiteSpace(profile.Name) ? username : profile.Name!;

        User user = User.Create(
            userCode,
            username,
            profile.Email,
            fullName,
            null,
            null,
            null,
            null,
            null
        );

        await users.AddAsync(user, cancellationToken);
        return user;
    }

    private async Task<string> GenerateUniqueUserCodeAsync(CancellationToken cancellationToken)
    {
        for (int attempt = 0; attempt < 16; attempt++)
        {
            string code = "U" + Guid.NewGuid().ToString("N")[..15].ToUpperInvariant();
            if (!await users.ExistsByUserCodeAsync(code, cancellationToken))
            {
                return code;
            }
        }

        throw new InvalidOperationException("Could not allocate a unique user code.");
    }

    private async Task<string> GenerateUniqueUsernameAsync(
        string email,
        CancellationToken cancellationToken
    )
    {
        string local = email.Split('@')[0];
        string normalized = new(local.Where(char.IsLetterOrDigit).ToArray());
        if (string.IsNullOrWhiteSpace(normalized))
        {
            normalized = "user";
        }

        string baseUsername = normalized[..Math.Min(normalized.Length, 24)];

        for (int attempt = 0; attempt < 100; attempt++)
        {
            string candidate = attempt == 0 ? baseUsername : $"{baseUsername}{attempt:D2}";
            if (!await users.ExistsByUsernameAsync(candidate, cancellationToken))
            {
                return candidate;
            }
        }

        string fallback = $"user{Guid.NewGuid():N}"[..20];
        if (!await users.ExistsByUsernameAsync(fallback, cancellationToken))
        {
            return fallback;
        }

        throw new FridayException(
            ErrorCodes.Admin.UserUsernameExists,
            "Could not allocate a unique username for OAuth2 sign-in."
        );
    }
}
