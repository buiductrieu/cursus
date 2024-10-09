using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class RfToken123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cb62845b-6ccb-4e19-8508-7f4086116aca");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f487bcf2-9707-44b6-97cc-618fe2cb171f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f9620a00-a100-42aa-b6a2-bdaba28883c0");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "20076290-9540-4164-8745-a18f1e91f002", null, "User", "USER" },
                    { "29a07f1a-3a5d-4726-91cc-76fa6d9bb4ce", null, "Instructor", "INSTRUCTOR" },
                    { "f8c1f96b-3a3a-446d-be98-dd684bd6e888", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "20076290-9540-4164-8745-a18f1e91f002");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "29a07f1a-3a5d-4726-91cc-76fa6d9bb4ce");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8c1f96b-3a3a-446d-be98-dd684bd6e888");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "cb62845b-6ccb-4e19-8508-7f4086116aca", null, "User", "USER" },
                    { "f487bcf2-9707-44b6-97cc-618fe2cb171f", null, "Admin", "ADMIN" },
                    { "f9620a00-a100-42aa-b6a2-bdaba28883c0", null, "Instructor", "INSTRUCTOR" }
                });
        }
    }
}
