using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friday.BuildingBlocks.Infrastructure.Migrations;

/// <inheritdoc />
public partial class RightsStructuredPermissions : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_rights_Code",
            schema: "admin",
            table: "rights");

        migrationBuilder.AddColumn<string>(
            name: "Module",
            schema: "admin",
            table: "rights",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Resource",
            schema: "admin",
            table: "rights",
            type: "character varying(120)",
            maxLength: 120,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AccessLevel",
            schema: "admin",
            table: "rights",
            type: "character varying(20)",
            maxLength: 20,
            nullable: true);

        // Map legacy flat codes (pre–structured model) to module + resource + coarse access level.
        migrationBuilder.Sql(
            """
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'dashboard', "AccessLevel" = 'read'
            WHERE UPPER(TRIM("Code")) = 'DASHBOARD_VIEW';
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'users', "AccessLevel" = 'read'
            WHERE UPPER(TRIM("Code")) = 'USERS_VIEW';
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'users', "AccessLevel" = 'manage'
            WHERE UPPER(TRIM("Code")) IN ('USERS_CREATE','USERS_EDIT','USERS_DELETE');
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'roles', "AccessLevel" = 'read'
            WHERE UPPER(TRIM("Code")) = 'ROLES_VIEW';
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'roles', "AccessLevel" = 'manage'
            WHERE UPPER(TRIM("Code")) = 'ROLES_EDIT';
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'permissions', "AccessLevel" = 'read'
            WHERE UPPER(TRIM("Code")) = 'RIGHTS_VIEW';
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'permissions', "AccessLevel" = 'manage'
            WHERE UPPER(TRIM("Code")) = 'RIGHTS_EDIT';
            UPDATE admin."rights" SET "Module" = 'admin', "Resource" = 'legacy', "AccessLevel" = 'read'
            WHERE "Module" IS NULL;
            """
        );

        // Point role_rights at one canonical right id per (Module, Resource, AccessLevel).
        migrationBuilder.Sql(
            """
            UPDATE admin."role_rights" AS rr
            SET "RightId" = c.keep_id
            FROM (
                SELECT "Module" AS m, "Resource" AS r, "AccessLevel" AS a, MIN("Id") AS keep_id
                FROM admin."rights"
                GROUP BY "Module", "Resource", "AccessLevel"
            ) AS c
            INNER JOIN admin."rights" AS r ON r."Id" = rr."RightId"
            WHERE r."Module" = c.m AND r."Resource" = c.r AND r."AccessLevel" = c.a
              AND rr."RightId" <> c.keep_id;

            DELETE FROM admin."role_rights" a
            USING admin."role_rights" b
            WHERE a."RoleId" = b."RoleId" AND a."RightId" = b."RightId" AND a.ctid < b.ctid;

            DELETE FROM admin."rights" r
            WHERE r."Id" NOT IN (
                SELECT MIN("Id") FROM admin."rights" GROUP BY "Module", "Resource", "AccessLevel"
            );
            """
        );

        migrationBuilder.DropColumn(name: "Code", schema: "admin", table: "rights");

        migrationBuilder.AlterColumn<string>(
            name: "Module",
            schema: "admin",
            table: "rights",
            type: "character varying(64)",
            maxLength: 64,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(64)",
            oldMaxLength: 64,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Resource",
            schema: "admin",
            table: "rights",
            type: "character varying(120)",
            maxLength: 120,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(120)",
            oldMaxLength: 120,
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "AccessLevel",
            schema: "admin",
            table: "rights",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(20)",
            oldMaxLength: 20,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_rights_Module_Resource_AccessLevel",
            schema: "admin",
            table: "rights",
            columns: new[] { "Module", "Resource", "AccessLevel" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_rights_Module_Resource_AccessLevel",
            schema: "admin",
            table: "rights");

        migrationBuilder.DropColumn(name: "Module", schema: "admin", table: "rights");
        migrationBuilder.DropColumn(name: "Resource", schema: "admin", table: "rights");
        migrationBuilder.DropColumn(name: "AccessLevel", schema: "admin", table: "rights");

        migrationBuilder.AddColumn<string>(
            name: "Code",
            schema: "admin",
            table: "rights",
            type: "character varying(120)",
            maxLength: 120,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateIndex(
            name: "IX_rights_Code",
            schema: "admin",
            table: "rights",
            column: "Code",
            unique: true);
    }
}
