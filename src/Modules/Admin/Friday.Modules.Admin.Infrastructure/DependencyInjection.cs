using Friday.Modules.Admin.Application.Configuration;
using Friday.Modules.Admin.Application.Security;
using Friday.Modules.Admin.Infrastructure.Security;
using Friday.Modules.Admin.Domain.Repositories;
using Friday.Modules.Admin.Domain.Security;
using Friday.Modules.Admin.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Admin.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<OAuth2Options>(configuration.GetSection(OAuth2Options.SectionName));
        services.Configure<EffectivePermissionCacheOptions>(
            configuration.GetSection(EffectivePermissionCacheOptions.SectionName)
        );
        services.AddSingleton<IEffectivePermissionGrantCacheCoordinator, EffectivePermissionGrantCacheCoordinator>();
        services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();
        services.AddHttpClient<IOAuth2ProfileClient, OAuth2ProfileClient>();
        services.AddScoped<IPasswordHasher<CredentialUser>, PasswordHasher<CredentialUser>>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRightRepository, RightRepository>();
        services.AddScoped<IEffectivePermissionResolver, EffectivePermissionResolver>();

        return services;
    }
}
