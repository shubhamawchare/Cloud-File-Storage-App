using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFileStorageApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicLinkSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "StoredFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ShareToken",
                table: "StoredFiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "StoredFiles");

            migrationBuilder.DropColumn(
                name: "ShareToken",
                table: "StoredFiles");
        }
    }
}
