using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friday.Modules.Salon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MChair_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "salon_admin_users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_admin_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_before_after",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    BeforeImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    AfterImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_before_after", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_gallery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_gallery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_promotions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SectionKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    Subtitle = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Body = table.Column<string>(type: "TEXT", nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsVisible = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_sections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    PriceFrom = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    RatingDisplay = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SettingKey = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_stylists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Bio = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_stylists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_testimonials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_testimonials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "salon_appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    HairServiceId = table.Column<int>(type: "INTEGER", nullable: false),
                    StylistId = table.Column<int>(type: "INTEGER", nullable: true),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_salon_appointments_salon_services_HairServiceId",
                        column: x => x.HairServiceId,
                        principalTable: "salon_services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_salon_appointments_salon_stylists_StylistId",
                        column: x => x.StylistId,
                        principalTable: "salon_stylists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_salon_admin_users_Username",
                table: "salon_admin_users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salon_appointments_HairServiceId",
                table: "salon_appointments",
                column: "HairServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_salon_appointments_ScheduledAt",
                table: "salon_appointments",
                column: "ScheduledAt");

            migrationBuilder.CreateIndex(
                name: "IX_salon_appointments_StylistId",
                table: "salon_appointments",
                column: "StylistId");

            migrationBuilder.CreateIndex(
                name: "IX_salon_sections_SectionKey",
                table: "salon_sections",
                column: "SectionKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salon_settings_SettingKey",
                table: "salon_settings",
                column: "SettingKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "salon_admin_users");

            migrationBuilder.DropTable(
                name: "salon_appointments");

            migrationBuilder.DropTable(
                name: "salon_before_after");

            migrationBuilder.DropTable(
                name: "salon_gallery");

            migrationBuilder.DropTable(
                name: "salon_promotions");

            migrationBuilder.DropTable(
                name: "salon_sections");

            migrationBuilder.DropTable(
                name: "salon_settings");

            migrationBuilder.DropTable(
                name: "salon_testimonials");

            migrationBuilder.DropTable(
                name: "salon_services");

            migrationBuilder.DropTable(
                name: "salon_stylists");
        }
    }
}
