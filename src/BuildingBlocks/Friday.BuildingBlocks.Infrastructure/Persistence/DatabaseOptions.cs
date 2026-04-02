namespace Friday.BuildingBlocks.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>
    /// When true, runs <see cref="Microsoft.EntityFrameworkCore.RelationalDatabaseFacadeExtensions.MigrateAsync"/> on the shared
    /// <see cref="FridayDbContext"/>, then FluentMigrator data migrations (same connection). Skipped for non-relational providers (e.g. in-memory).
    /// </summary>
    public bool ApplyMigrationsOnStartup { get; set; }
}
