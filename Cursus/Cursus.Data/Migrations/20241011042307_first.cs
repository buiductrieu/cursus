using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5297f3da-ae8d-4713-901e-fca47bccaaf9", null, "Admin", "ADMIN" },
                    { "a5a25442-4d0b-491f-878d-5df622c2698f", null, "User", "USER" },
                    { "ed480cfe-3b5e-4508-9b9d-da13127c2630", null, "Instructor", "INSTRUCTOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5297f3da-ae8d-4713-901e-fca47bccaaf9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5a25442-4d0b-491f-878d-5df622c2698f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ed480cfe-3b5e-4508-9b9d-da13127c2630");

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
