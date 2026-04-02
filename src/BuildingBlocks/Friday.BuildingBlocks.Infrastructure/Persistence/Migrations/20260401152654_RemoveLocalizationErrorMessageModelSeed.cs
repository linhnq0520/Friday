using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friday.BuildingBlocks.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class RemoveLocalizationErrorMessageModelSeed : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Intentionally empty: <c>localization.error_messages</c> is no longer model-seeded via <c>HasData</c>.
        // Rows created by earlier migrations (if any) stay in the database; runtime seeders upsert by natural key
        // (Module, ErrorCode, Language). Use <c>Database:ApplyMigrationsOnStartup</c> or FluentMigrator in a release job.
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // No-op: do not reintroduce migration-owned seed; use FluentMigrator data migrations instead.
    }
}
