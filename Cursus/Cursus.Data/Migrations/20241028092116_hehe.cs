using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cursus.Data.Migrations
{
    /// <inheritdoc />
    public partial class hehe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "PayoutRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstructorId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayoutRequestStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayoutRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayoutRequests_InstructorInfos_InstructorId",
                        column: x => x.InstructorId,
                        principalTable: "InstructorInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayoutRequests_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "TransactionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0b4d0048-24bf-45fb-8a71-254e9968cdf8", null, "Instructor", "INSTRUCTOR" },
                    { "58d269d8-f848-4515-818b-08a70f48cdbe", null, "Admin", "ADMIN" },
                    { "86bc2887-0779-4b23-b7f8-c87e5ea53e48", null, "User", "USER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PayoutRequests_InstructorId",
                table: "PayoutRequests",
                column: "InstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_PayoutRequests_TransactionId",
                table: "PayoutRequests",
                column: "TransactionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PayoutRequests");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0b4d0048-24bf-45fb-8a71-254e9968cdf8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "58d269d8-f848-4515-818b-08a70f48cdbe");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "86bc2887-0779-4b23-b7f8-c87e5ea53e48");

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
    }
}
