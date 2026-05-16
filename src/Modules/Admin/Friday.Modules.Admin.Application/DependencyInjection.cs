using FluentValidation;
using Friday.Modules.Admin.Application.Features.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Admin.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAdminApplication(this IServiceCollection services)
    {
        services.AddScoped<IValidator<LoginCommand>, LoginCommandValidator>();

        return services;
    }
}
