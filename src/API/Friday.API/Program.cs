using Friday.API.Common;
using Friday.API.Middlewares;
using Friday.API.Modules.Admin;
using Friday.API.Modules.Sample;
using Friday.BuildingBlocks.Application;
using Friday.BuildingBlocks.Application.Cqrs;
using Friday.BuildingBlocks.Application.IntegrationEvents;
using Friday.BuildingBlocks.Infrastructure;
using Friday.Modules.Admin.Application;
using Friday.Modules.Admin.Infrastructure;
using Friday.Modules.Sample.Application;
using Friday.Modules.Sample.Infrastructure;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog(
        (services, loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services)
                .Enrich.WithProperty("Application", "Friday.API")
    );

    builder.Services.AddBuildingBlocksApplication();
    builder.Services.AddBuildingBlocksInfrastructure(builder.Configuration);
    builder.Services.AddMemoryCache();
    builder.Services.Configure<LocalizationOptions>(
        builder.Configuration.GetSection("Localization")
    );
    builder.Services.AddScoped<IErrorMessageLocalizer, ErrorMessageLocalizer>();
    builder.Services.AddAdminApplication();
    builder.Services.AddAdminInfrastructure(builder.Configuration);
    builder.Services.AddSampleApplication();
    builder.Services.AddSampleInfrastructure();
    builder.Services.AddIntegrationEventsRouting(registry =>
        registry.Register<UserCreatedIntegrationEvent>(ModuleKeys.Sample)
    );

    WebApplication app = builder.Build();

    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
        };

        options.GetLevel = (httpContext, elapsed, exception) =>
        {
            if (exception is not null || httpContext.Response.StatusCode >= 500)
            {
                return LogEventLevel.Error;
            }

            if (elapsed > 500)
            {
                return LogEventLevel.Warning;
            }

            return LogEventLevel.Information;
        };
    });
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    app.UseHttpsRedirection();

    app.MapGet(
        "/",
        (HttpContext context) => ApiResults.Ok(context, "Friday modular monolith is running.")
    );
    app.MapAdminModule();
    app.MapSampleModule();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Server terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}
