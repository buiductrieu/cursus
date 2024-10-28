using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class hehehehe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8f8ac768-9583-4fac-bd4b-8a2a1a059cae");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90c9776e-8ac9-475d-9ff9-c2d8dff0a936");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eac39dbd-d929-4465-8d1b-6ed4d9031f2f");

            migrationBuilder.AddColumn<double>(
                name: "TotalWithdrawn",
                table: "InstructorInfos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "WithdrawalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PayoutRequestStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithdrawalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithdrawalHistories_InstructorInfos_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "InstructorInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1be101cf-9477-437d-8ab8-9c0b2695dbc5", null, "Admin", "ADMIN" },
                    { "ba664d48-44ff-4227-bc58-c07ee5d8ab16", null, "User", "USER" },
                    { "ff66cce5-5079-44ba-8ee0-4098879fb51b", null, "Instructor", "INSTRUCTOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WithdrawalHistories_InstructorId",
                table: "WithdrawalHistories",
                column: "InstructorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WithdrawalHistories");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1be101cf-9477-437d-8ab8-9c0b2695dbc5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ba664d48-44ff-4227-bc58-c07ee5d8ab16");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ff66cce5-5079-44ba-8ee0-4098879fb51b");

            migrationBuilder.DropColumn(
                name: "TotalWithdrawn",
                table: "InstructorInfos");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "8f8ac768-9583-4fac-bd4b-8a2a1a059cae", null, "Admin", "ADMIN" },
                    { "90c9776e-8ac9-475d-9ff9-c2d8dff0a936", null, "Instructor", "INSTRUCTOR" },
                    { "eac39dbd-d929-4465-8d1b-6ed4d9031f2f", null, "User", "USER" }
                });
        }
    }
}
