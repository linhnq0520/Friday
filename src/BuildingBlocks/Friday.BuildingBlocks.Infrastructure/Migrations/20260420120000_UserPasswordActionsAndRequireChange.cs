using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friday.BuildingBlocks.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UserPasswordActionsAndRequireChange : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "RequirePasswordChange",
            schema: "admin",
            table: "users",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "user_password_actions",
            schema: "admin",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<int>(type: "integer", nullable: false),
                ActionType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                ExpiresAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                CreatedOnUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ConsumedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_password_actions", x => x.Id);
                table.ForeignKey(
                    name: "FK_user_password_actions_users_UserId",
                    column: x => x.UserId,
                    principalSchema: "admin",
                    principalTable: "users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_user_password_actions_TokenHash",
            schema: "admin",
            table: "user_password_actions",
            column: "TokenHash",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_user_password_actions_UserId_ActionType",
            schema: "admin",
            table: "user_password_actions",
            columns: new[] { "UserId", "ActionType" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "user_password_actions",
            schema: "admin");

        migrationBuilder.DropColumn(
            name: "RequirePasswordChange",
            schema: "admin",
            table: "users");
    }
}
