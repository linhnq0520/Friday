using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Salon.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddSalonApplication(this IServiceCollection services)
    {
        services.AddScoped<Security.IAdminPasswordService, Security.AdminPasswordService>();
        return services;
    }
}
