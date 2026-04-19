using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.API.Authorization;

public static class AdminPermissionAuthorizationRegistration
{
    /// <summary>
    /// Registers dynamic permission policies and the authorization handler (call after AddAuthorization).
    /// </summary>
    public static IServiceCollection AddAdminPermissionAuthorization(
        this IServiceCollection services
    )
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}
