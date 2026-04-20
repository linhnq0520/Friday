using Friday.Modules.Integration.Application.Abstractions;
using Friday.Modules.Integration.Application.Configuration;
using Friday.Modules.Integration.Domain.Repositories;
using Friday.Modules.Integration.Infrastructure.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Integration.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIntegrationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.AddScoped<IUserCredentialNotificationService, UserCredentialNotificationService>();
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        return services;
    }
}
