using Friday.BuildingBlocks.Application.Abstractions;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Friday.Modules.Salon.Domain.Repositories;
using Friday.Modules.Salon.Infrastructure.Persistence;
using Friday.Modules.Salon.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.Modules.Salon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddSalonInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<DatabaseOptions>(configuration.GetSection(DatabaseOptions.SectionName));

        DatabaseOptions dbSettings =
            configuration.GetSection(DatabaseOptions.SectionName).Get<DatabaseOptions>()
            ?? new DatabaseOptions { Provider = RelationalDatabaseProvider.Sqlite };

        string? connectionString = configuration.GetConnectionString("FridayDb");
        if (
            dbSettings.Provider == RelationalDatabaseProvider.Sqlite
            && !string.IsNullOrWhiteSpace(connectionString)
        )
        {
            EnsureSqliteDirectory(connectionString);
        }

        services.AddDbContext<SalonDbContext>(options =>
            RelationalDbContextConfigurer.Configure(options, connectionString, dbSettings.Provider)
        );

        services.AddScoped<IUnitOfWork, SalonUnitOfWork>();
        services.AddScoped<ISalonRepository, SalonRepository>();
        return services;
    }

    private static void EnsureSqliteDirectory(string connectionString)
    {
        const string prefix = "Data Source=";
        int index = connectionString.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return;
        }

        string path = connectionString[(index + prefix.Length)..].Trim().Trim('"');
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}
