using System.Net.Http.Json;
using System.Text.Json;
using Friday.BuildingBlocks.Application.Errors;
using Friday.BuildingBlocks.Application.Exceptions;
using Friday.Modules.Admin.Application.Configuration;
using Friday.Modules.Admin.Application.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Friday.Modules.Admin.Infrastructure.Security;

public sealed class OAuth2ProfileClient(HttpClient httpClient, IOptions<OAuth2Options> options)
    : IOAuth2ProfileClient
{
    public async Task<OAuth2UserProfile> ExchangeCodeForProfileAsync(
        string provider,
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(provider))
        {
            throw new FridayException(ErrorCodes.Admin.OAuth2ProviderInvalid, "OAuth2 provider is required.");
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2AuthorizationCodeInvalid,
                "OAuth2 authorization code is required.",
                StatusCodes.Status400BadRequest
            );
        }

        OAuth2ProviderOptions providerOptions = ResolveProvider(provider);

        string accessToken = await ExchangeCodeAsync(
            providerOptions,
            code,
            redirectUri,
            cancellationToken
        );

        OAuth2UserProfile profile = await FetchProfileAsync(
            provider,
            providerOptions,
            accessToken,
            cancellationToken
        );

        return profile;
    }

    private OAuth2ProviderOptions ResolveProvider(string provider)
    {
        if (!options.Value.Providers.TryGetValue(provider, out OAuth2ProviderOptions? providerOptions))
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2ProviderInvalid,
                $"OAuth2 provider '{provider}' is not configured.",
                StatusCodes.Status400BadRequest
            );
        }

        if (!providerOptions.Enabled)
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2ProviderDisabled,
                $"OAuth2 provider '{provider}' is disabled.",
                StatusCodes.Status403Forbidden
            );
        }

        if (
            string.IsNullOrWhiteSpace(providerOptions.ClientId)
            || string.IsNullOrWhiteSpace(providerOptions.ClientSecret)
            || string.IsNullOrWhiteSpace(providerOptions.TokenEndpoint)
            || string.IsNullOrWhiteSpace(providerOptions.UserInfoEndpoint)
        )
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2ProviderInvalid,
                $"OAuth2 provider '{provider}' is missing required settings.",
                StatusCodes.Status500InternalServerError
            );
        }

        return providerOptions;
    }

    private async Task<string> ExchangeCodeAsync(
        OAuth2ProviderOptions providerOptions,
        string code,
        string redirectUri,
        CancellationToken cancellationToken
    )
    {
        Dictionary<string, string> form = new()
        {
            ["grant_type"] = "authorization_code",
            ["client_id"] = providerOptions.ClientId,
            ["client_secret"] = providerOptions.ClientSecret,
            ["code"] = code,
            ["redirect_uri"] = redirectUri,
        };

        if (!string.IsNullOrWhiteSpace(providerOptions.Scope))
        {
            form["scope"] = providerOptions.Scope;
        }

        using HttpResponseMessage response = await httpClient.PostAsync(
            providerOptions.TokenEndpoint,
            new FormUrlEncodedContent(form),
            cancellationToken
        );

        if (!response.IsSuccessStatusCode)
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2TokenExchangeFailed,
                "OAuth2 token exchange failed.",
                StatusCodes.Status401Unauthorized
            );
        }

        JsonElement tokenJson = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);
        if (
            !tokenJson.TryGetProperty("access_token", out JsonElement accessTokenElement)
            || string.IsNullOrWhiteSpace(accessTokenElement.GetString())
        )
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2TokenExchangeFailed,
                "OAuth2 token response does not contain access_token.",
                StatusCodes.Status401Unauthorized
            );
        }

        return accessTokenElement.GetString()!;
    }

    private async Task<OAuth2UserProfile> FetchProfileAsync(
        string provider,
        OAuth2ProviderOptions providerOptions,
        string accessToken,
        CancellationToken cancellationToken
    )
    {
        using HttpRequestMessage request = new(HttpMethod.Get, providerOptions.UserInfoEndpoint);
        request.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        using HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2UserInfoFailed,
                "OAuth2 user info request failed.",
                StatusCodes.Status401Unauthorized
            );
        }

        JsonElement profileJson = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken);

        string? subject = ReadString(profileJson, providerOptions.SubjectField);
        string? email = ReadString(profileJson, providerOptions.EmailField);
        string? name = ReadString(profileJson, providerOptions.NameField);

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2UserInfoFailed,
                "OAuth2 user info does not contain subject.",
                StatusCodes.Status401Unauthorized
            );
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new FridayException(
                ErrorCodes.Admin.OAuth2EmailMissing,
                "OAuth2 account does not provide an email.",
                StatusCodes.Status400BadRequest
            );
        }

        return new OAuth2UserProfile(provider, subject, email, name);
    }

    private static string? ReadString(JsonElement json, string propertyName)
    {
        if (!json.TryGetProperty(propertyName, out JsonElement value))
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
    }
}
