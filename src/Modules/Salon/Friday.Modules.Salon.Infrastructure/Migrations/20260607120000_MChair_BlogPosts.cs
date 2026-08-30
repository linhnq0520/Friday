using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friday.Modules.Salon.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MChair_BlogPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "salon_blog_posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Summary = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AuthorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsPublished = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    ViewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    MetaTitle = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    MetaDescription = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    MetaKeywords = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOnUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salon_blog_posts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_salon_blog_posts_Slug",
                table: "salon_blog_posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_salon_blog_posts_Category",
                table: "salon_blog_posts",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_salon_blog_posts_IsPublished",
                table: "salon_blog_posts",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_salon_blog_posts_PublishedAt",
                table: "salon_blog_posts",
                column: "PublishedAt");

            migrationBuilder.CreateIndex(
                name: "IX_salon_blog_posts_IsFeatured",
                table: "salon_blog_posts",
                column: "IsFeatured");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "salon_blog_posts");
        }
    }
}
