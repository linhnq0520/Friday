namespace Friday.Modules.Admin.Application.Security;

public interface IOAuth2ProfileClient
{
    Task<OAuth2UserProfile> ExchangeCodeForProfileAsync(
        string provider,
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default
    );
}

public sealed record OAuth2UserProfile(string Provider, string Subject, string Email, string? Name);
