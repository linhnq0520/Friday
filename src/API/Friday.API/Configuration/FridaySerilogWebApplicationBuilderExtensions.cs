using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

namespace Friday.API.Configuration;

public static class FridaySerilogWebApplicationBuilderExtensions
{
    /// <summary>
    /// Cấu hình Serilog qua <c>Host.UseSerilog</c> (một nguồn duy nhất): console từ <c>SerilogClassic</c>; file JSON dưới content root <c>logs/</c>.
    /// </summary>
    public static WebApplicationBuilder AddFridaySerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(
            (HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
            {
                IHostEnvironment env = context.HostingEnvironment;
                IConfiguration configuration = context.Configuration;

                OpenTelemetryOptions otel =
                    configuration.GetSection(OpenTelemetryOptions.SectionName).Get<OpenTelemetryOptions>()
                    ?? new OpenTelemetryOptions();

                loggerConfiguration
                    .ReadFrom.Configuration(configuration.GetSection("Serilog"))
                    .ReadFrom.Services(services)
                    .Enrich.WithProperty("Application", "Friday.API");

                bool otelLogs = string.Equals(
                    otel.LogExport,
                    LogExportModes.OpenTelemetry,
                    StringComparison.OrdinalIgnoreCase
                );

                if (
                    otelLogs
                    && !string.IsNullOrWhiteSpace(otel.OtlpEndpoint)
                    && Uri.TryCreate(otel.OtlpEndpoint, UriKind.Absolute, out _)
                )
                {
                    loggerConfiguration.WriteTo.OpenTelemetry(o =>
                    {
                        o.Endpoint = otel.OtlpEndpoint;
                        o.ResourceAttributes["service.name"] = otel.ServiceName;
                    });
                }
                else
                {
                    loggerConfiguration.ReadFrom.Configuration(
                        configuration.GetSection("SerilogClassic")
                    );
                }

                AppendRollingJsonFile(loggerConfiguration, env.ContentRootPath);
                AppendGrafanaLoki(loggerConfiguration, configuration, otel);
            }
        );

        return builder;
    }

    private static void AppendRollingJsonFile(LoggerConfiguration loggerConfiguration, string contentRoot)
    {
        string logsDir = Path.Combine(contentRoot, "logs");
        Directory.CreateDirectory(logsDir);
        string path = Path.Combine(logsDir, "log-.json");

        loggerConfiguration.WriteTo.File(
            new CompactJsonFormatter(),
            path,
            rollingInterval: RollingInterval.Day,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 104_857_600,
            retainedFileCountLimit: 14,
            shared: true,
            flushToDiskInterval: TimeSpan.FromSeconds(1)
        );
    }

    private static void AppendGrafanaLoki(
        LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        OpenTelemetryOptions otel
    )
    {
        GrafanaLokiOptions loki =
            configuration.GetSection(GrafanaLokiOptions.SectionName).Get<GrafanaLokiOptions>()
            ?? new GrafanaLokiOptions();

        if (!loki.Enabled || string.IsNullOrWhiteSpace(loki.Uri))
        {
            return;
        }

        if (!Uri.TryCreate(loki.Uri.Trim(), UriKind.Absolute, out Uri? lokiUri))
        {
            return;
        }

        string serviceName = string.IsNullOrWhiteSpace(otel.ServiceName) ? "Friday.API" : otel.ServiceName;

        loggerConfiguration.WriteTo.GrafanaLoki(
            lokiUri.GetLeftPart(UriPartial.Authority),
            [new LokiLabel { Key = "service_name", Value = serviceName }]
        );
    }
}
