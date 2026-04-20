namespace Friday.Modules.Admin.Application.Configuration;

public sealed class OAuth2Options
{
    public const string SectionName = "Authentication:OAuth2";

    public string DefaultRoleCode { get; set; } = "USER";

    public Dictionary<string, OAuth2ProviderOptions> Providers { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}

public sealed class OAuth2ProviderOptions
{
    public bool Enabled { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string TokenEndpoint { get; set; } = string.Empty;

    public string UserInfoEndpoint { get; set; } = string.Empty;

    public string? Scope { get; set; }

    public string SubjectField { get; set; } = "sub";

    public string EmailField { get; set; } = "email";

    public string NameField { get; set; } = "name";
}
