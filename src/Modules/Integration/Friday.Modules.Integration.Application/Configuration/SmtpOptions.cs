namespace Friday.Modules.Integration.Application.Configuration;

public sealed class SmtpOptions
{
    public const string SectionName = "Integration:Email:Smtp";

    public bool Enabled { get; set; }
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string FromAddress { get; set; } = "noreply@friday.local";
    public string FromName { get; set; } = "Friday";
}
