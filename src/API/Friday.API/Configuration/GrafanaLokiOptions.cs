namespace Friday.API.Configuration;

public sealed class GrafanaLokiOptions
{
    /// <summary>Maps to <c>Serilog:GrafanaLoki</c> in configuration (JSON: <c>Serilog.GrafanaLoki</c>).</summary>
    public const string SectionName = "Serilog:GrafanaLoki";

    public bool Enabled { get; set; }

    /// <summary>Base URL Loki, ví dụ <c>http://localhost:3100</c> hoặc <c>http://loki:3100</c> trong Docker.</summary>
    public string? Uri { get; set; }
}
