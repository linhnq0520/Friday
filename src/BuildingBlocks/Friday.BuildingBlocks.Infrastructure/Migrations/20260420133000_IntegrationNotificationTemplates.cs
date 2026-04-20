using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Friday.BuildingBlocks.Infrastructure.Migrations;

/// <inheritdoc />
public partial class IntegrationNotificationTemplates : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(name: "integration");

        migrationBuilder.CreateTable(
            name: "notification_templates",
            schema: "integration",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Channel = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                TemplateCode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                SubjectTemplate = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                BodyTemplate = table.Column<string>(type: "text", nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                Version = table.Column<int>(type: "integer", nullable: false),
                UpdatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_notification_templates", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_notification_templates_Channel_TemplateCode_Language_IsActive",
            schema: "integration",
            table: "notification_templates",
            columns: new[] { "Channel", "TemplateCode", "Language", "IsActive" });

        migrationBuilder.CreateIndex(
            name: "IX_notification_templates_Channel_TemplateCode_Language_Version",
            schema: "integration",
            table: "notification_templates",
            columns: new[] { "Channel", "TemplateCode", "Language", "Version" },
            unique: true);

        migrationBuilder.InsertData(
            schema: "integration",
            table: "notification_templates",
            columns: new[]
            {
                "Channel",
                "TemplateCode",
                "Language",
                "SubjectTemplate",
                "BodyTemplate",
                "IsActive",
                "Version",
                "UpdatedOnUtc",
            },
            values: new object[,]
            {
                {
                    "email",
                    "USER_TEMP_PASSWORD",
                    "en",
                    "Friday account credentials",
                    "Hello {{FullName}},\n\nYour account has been created by an administrator.\nTemporary password: {{TemporaryPassword}}\nYou must change this password after your first sign-in.\n",
                    true,
                    1,
                    DateTime.UtcNow,
                },
                {
                    "email",
                    "PASSWORD_RESET",
                    "en",
                    "Friday password reset",
                    "Hello {{FullName}},\n\nWe received a request to reset your password.\nReset link: {{ResetLink}}\nThis link expires at {{ExpiresAtUtc}}.\n",
                    true,
                    1,
                    DateTime.UtcNow,
                },
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "notification_templates",
            schema: "integration");
    }
}
