using System;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Sqlite.Migrations.Internal;

namespace Friday.BuildingBlocks.Infrastructure.Persistence;

#pragma warning disable EF1001

public class SafeSqliteMigrationsSqlGenerator : SqliteMigrationsSqlGenerator
{
    private static readonly Regex CreateTableRegex = new(
        @"^\s*CREATE\s+TABLE\s+(?!IF\s+NOT\s+EXISTS\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex CreateUniqueIndexRegex = new(
        @"^\s*CREATE\s+UNIQUE\s+INDEX\s+(?!IF\s+NOT\s+EXISTS\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex CreateIndexRegex = new(
        @"^\s*CREATE\s+INDEX\s+(?!IF\s+NOT\s+EXISTS\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex DropTableRegex = new(
        @"^\s*DROP\s+TABLE\s+(?!IF\s+EXISTS\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    private static readonly Regex DropIndexRegex = new(
        @"^\s*DROP\s+INDEX\s+(?!IF\s+EXISTS\b)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    public SafeSqliteMigrationsSqlGenerator(
        MigrationsSqlGeneratorDependencies dependencies,
        IRelationalAnnotationProvider relationalAnnotationProvider
    ) : base(dependencies, relationalAnnotationProvider)
    {
    }

    protected override void Generate(
        CreateTableOperation operation,
        IModel? model,
        MigrationCommandListBuilder builder,
        bool terminate = true
    )
    {
        // Let EF Core internal tables (e.g. __EFMigrationsHistory) be generated as-is,
        // because SqliteHistoryRepository.GetCreateIfNotExistsScript() adds " IF NOT EXISTS" manually.
        if (operation.Name.StartsWith("__EF", StringComparison.OrdinalIgnoreCase))
        {
            base.Generate(operation, model, builder, terminate);
            return;
        }

        MigrationCommandListBuilder subBuilder = new(Dependencies);
        base.Generate(operation, model, subBuilder, terminate);

        foreach (MigrationCommand command in subBuilder.GetCommandList())
        {
            string commandText = CreateTableRegex.Replace(command.CommandText, "CREATE TABLE IF NOT EXISTS ");
            builder.Append(commandText);
            if (terminate)
            {
                builder.EndCommand();
            }
        }
    }

    protected override void Generate(
        CreateIndexOperation operation,
        IModel? model,
        MigrationCommandListBuilder builder,
        bool terminate = true
    )
    {
        if (operation.Table?.StartsWith("__EF", StringComparison.OrdinalIgnoreCase) == true)
        {
            base.Generate(operation, model, builder, terminate);
            return;
        }

        MigrationCommandListBuilder subBuilder = new(Dependencies);
        base.Generate(operation, model, subBuilder, terminate);

        foreach (MigrationCommand command in subBuilder.GetCommandList())
        {
            string commandText = command.CommandText;
            if (CreateUniqueIndexRegex.IsMatch(commandText))
            {
                commandText = CreateUniqueIndexRegex.Replace(commandText, "CREATE UNIQUE INDEX IF NOT EXISTS ");
            }
            else if (CreateIndexRegex.IsMatch(commandText))
            {
                commandText = CreateIndexRegex.Replace(commandText, "CREATE INDEX IF NOT EXISTS ");
            }

            builder.Append(commandText);
            if (terminate)
            {
                builder.EndCommand();
            }
        }
    }

    protected override void Generate(
        DropTableOperation operation,
        IModel? model,
        MigrationCommandListBuilder builder,
        bool terminate = true
    )
    {
        if (operation.Name.StartsWith("__EF", StringComparison.OrdinalIgnoreCase))
        {
            base.Generate(operation, model, builder, terminate);
            return;
        }

        MigrationCommandListBuilder subBuilder = new(Dependencies);
        base.Generate(operation, model, subBuilder, terminate);

        foreach (MigrationCommand command in subBuilder.GetCommandList())
        {
            string commandText = DropTableRegex.Replace(command.CommandText, "DROP TABLE IF EXISTS ");
            builder.Append(commandText);
            if (terminate)
            {
                builder.EndCommand();
            }
        }
    }

    protected override void Generate(
        DropIndexOperation operation,
        IModel? model,
        MigrationCommandListBuilder builder,
        bool terminate = true
    )
    {
        if (operation.Table?.StartsWith("__EF", StringComparison.OrdinalIgnoreCase) == true)
        {
            base.Generate(operation, model, builder, terminate);
            return;
        }

        MigrationCommandListBuilder subBuilder = new(Dependencies);
        base.Generate(operation, model, subBuilder, terminate);

        foreach (MigrationCommand command in subBuilder.GetCommandList())
        {
            string commandText = DropIndexRegex.Replace(command.CommandText, "DROP INDEX IF EXISTS ");
            builder.Append(commandText);
            if (terminate)
            {
                builder.EndCommand();
            }
        }
    }
}
