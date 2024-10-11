using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateBookmarksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "522a4cf3-ef59-432e-bdf6-07d3c881ae0f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0391d08-c897-41cf-a356-9ef62a3051ec");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbffcf5d-f40d-4252-b641-dd04176d8e22");

            migrationBuilder.CreateTable(
                name: "Bookmarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookmarks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "01767e72-47cd-4fbe-973f-9890eb6710be", null, "Admin", "ADMIN" },
                    { "bd9a8881-a20f-481c-9c1e-dcea5e787f92", null, "User", "USER" },
                    { "f918f6f1-f00f-43f9-a248-545b3c8f9986", null, "Instructor", "INSTRUCTOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_CourseId",
                table: "Bookmarks",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookmarks");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "01767e72-47cd-4fbe-973f-9890eb6710be");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd9a8881-a20f-481c-9c1e-dcea5e787f92");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f918f6f1-f00f-43f9-a248-545b3c8f9986");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "522a4cf3-ef59-432e-bdf6-07d3c881ae0f", null, "Instructor", "INSTRUCTOR" },
                    { "a0391d08-c897-41cf-a356-9ef62a3051ec", null, "User", "USER" },
                    { "cbffcf5d-f40d-4252-b641-dd04176d8e22", null, "Admin", "ADMIN" }
                });
        }
    }
}
