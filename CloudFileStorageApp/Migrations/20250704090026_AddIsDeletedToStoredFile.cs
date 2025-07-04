using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CloudFileStorageApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToStoredFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "StoredFiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StoredFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "StoredFiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StoredFiles");
        }
    }
}
