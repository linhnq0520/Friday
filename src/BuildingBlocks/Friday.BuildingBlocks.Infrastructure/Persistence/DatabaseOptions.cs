namespace Friday.BuildingBlocks.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>
    /// When true, runs all registered <c>IDataSeeder</c> implementations after the host starts.
    /// Prefer false in production and run seeding from CI/release jobs unless you intentionally bootstrap reference data from the app.
    /// </summary>
    public bool SeedOnStartup { get; set; }
}
