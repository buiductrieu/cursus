using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class StatusInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "StatusInsructor",
                table: "InstructorInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "StatusInsructor",
                table: "InstructorInfos");

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
    }
}
