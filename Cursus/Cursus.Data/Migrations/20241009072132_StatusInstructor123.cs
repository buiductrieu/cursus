using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatusInstructor123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "68fd531d-ab51-41fe-807d-3cd9206d7020");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a28db0c3-6665-493a-9228-d893e11d3bb4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c4823614-0619-46e8-823a-90cbc19daa5b");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "323744c1-37f3-4e3f-9e81-6edc633a9f49", null, "Instructor", "INSTRUCTOR" },
                    { "8c443767-e974-4e80-987c-c12198790864", null, "Admin", "ADMIN" },
                    { "8ddd5d04-9609-46b3-b987-57fd967e5666", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "323744c1-37f3-4e3f-9e81-6edc633a9f49");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c443767-e974-4e80-987c-c12198790864");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ddd5d04-9609-46b3-b987-57fd967e5666");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "68fd531d-ab51-41fe-807d-3cd9206d7020", null, "Admin", "ADMIN" },
                    { "a28db0c3-6665-493a-9228-d893e11d3bb4", null, "User", "USER" },
                    { "c4823614-0619-46e8-823a-90cbc19daa5b", null, "Instructor", "INSTRUCTOR" }
                });
        }
    }
}
