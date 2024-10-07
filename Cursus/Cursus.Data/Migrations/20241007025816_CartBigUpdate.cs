using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class CartBigUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "43b39c2d-7693-436a-848b-c7b20b45168f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ab2c0e0a-e21c-48fc-a880-afdb43c5069a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e9b1dbd7-09ce-441f-8e13-1107b0e7ea51");

            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0867cf46-5d02-4048-8a27-93999ecaae4d", null, "User", "USER" },
                    { "8fe77b6e-6305-4bc9-b8fd-90255f791855", null, "Admin", "ADMIN" },
                    { "f2d4be74-eb31-4d32-a2ba-66c49ac7ca43", null, "Instructor", "INSTRUCTOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0867cf46-5d02-4048-8a27-93999ecaae4d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8fe77b6e-6305-4bc9-b8fd-90255f791855");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f2d4be74-eb31-4d32-a2ba-66c49ac7ca43");

            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "43b39c2d-7693-436a-848b-c7b20b45168f", null, "Admin", "ADMIN" },
                    { "ab2c0e0a-e21c-48fc-a880-afdb43c5069a", null, "Instructor", "INSTRUCTOR" },
                    { "e9b1dbd7-09ce-441f-8e13-1107b0e7ea51", null, "User", "USER" }
                });
        }
    }
}
