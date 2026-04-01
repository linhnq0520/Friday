using Friday.BuildingBlocks.Application.Abstractions;
using Friday.BuildingBlocks.Application.Localization;
using Friday.BuildingBlocks.Infrastructure.Localization;
using Friday.BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Friday.BuildingBlocks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocksInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        string? connectionString = configuration.GetConnectionString("FridayDb");

        services.AddScoped<ILinqToDbConnectionFactory>(
            _ =>
                new LinqToDbConnectionFactory(
                    connectionString
                    ?? "Server=(localdb)\\MSSQLLocalDB;Database=FridayDb;Trusted_Connection=True;TrustServerCertificate=True;"
                )
        );
        services.AddDbContext<FridayDbContext>(options =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase("Friday.Shared");
            }
        });
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IErrorLocalizationStore, EfErrorLocalizationStore>();
        return services;
    }
}
