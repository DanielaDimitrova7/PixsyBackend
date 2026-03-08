using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PixsyAPI.Migrations
{
    public partial class AddPictureLikesAndComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PictureComments",
                columns: table => new
                {
                    PictureCommentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PictureID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictureComments", x => x.PictureCommentID);
                });

            migrationBuilder.CreateTable(
                name: "PictureLikes",
                columns: table => new
                {
                    PictureLikeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PictureID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PictureLikes", x => x.PictureLikeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PictureComments_PictureID",
                table: "PictureComments",
                column: "PictureID");

            migrationBuilder.CreateIndex(
                name: "IX_PictureLikes_PictureID",
                table: "PictureLikes",
                column: "PictureID");

            migrationBuilder.CreateIndex(
                name: "IX_PictureLikes_PictureID_UserID",
                table: "PictureLikes",
                columns: new[] { "PictureID", "UserID" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PictureComments");
            migrationBuilder.DropTable(name: "PictureLikes");
        }
    }
}
