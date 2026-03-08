using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixsyAPI.Migrations
{
    public partial class AddPictureFileMetadata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Pictures",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2026, 3, 8, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ContentType", table: "Pictures");
            migrationBuilder.DropColumn(name: "CreatedAtUtc", table: "Pictures");
            migrationBuilder.DropColumn(name: "ImagePath", table: "Pictures");
            migrationBuilder.DropColumn(name: "OriginalFileName", table: "Pictures");
        }
    }
}
