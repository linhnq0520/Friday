using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Friday.BuildingBlocks.Infrastructure.Persistence;

public sealed class FridayDbContext(DbContextOptions<FridayDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Assembly[] assemblies = AppDomain
            .CurrentDomain.GetAssemblies()
            .Where(x => x.GetName().Name?.StartsWith("Friday.") == true)
            .ToArray();

        foreach (Assembly assembly in assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
