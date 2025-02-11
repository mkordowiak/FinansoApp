using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinansoData.Migrations
{
    /// <inheritdoc />
    public partial class balancelogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BalanceLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    BalanceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BalanceLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BalanceLog_Balances_BalanceId",
                        column: x => x.BalanceId,
                        principalTable: "Balances",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BalanceLog_BalanceId_Date",
                table: "BalanceLog",
                columns: new[] { "BalanceId", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BalanceLog");
        }
    }
}
