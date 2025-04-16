using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace balcheckcalcweb.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CheckHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAlias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PolicyCount = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CheckHistoryDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CheckHistoryId = table.Column<int>(type: "int", nullable: false),
                    PolicyNumber = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Installment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevisedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckHistoryDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckHistoryDetails_CheckHistories_CheckHistoryId",
                        column: x => x.CheckHistoryId,
                        principalTable: "CheckHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CheckHistoryDetails_CheckHistoryId",
                table: "CheckHistoryDetails",
                column: "CheckHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CheckHistoryDetails");

            migrationBuilder.DropTable(
                name: "CheckHistories");
        }
    }
}
