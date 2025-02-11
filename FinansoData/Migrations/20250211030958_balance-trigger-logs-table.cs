using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinansoData.Migrations
{
    /// <inheritdoc />
    public partial class balancetriggerlogstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceLog_Balances_BalanceId",
                table: "BalanceLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BalanceLog",
                table: "BalanceLog");

            migrationBuilder.RenameTable(
                name: "BalanceLog",
                newName: "BalanceLogs");

            migrationBuilder.RenameIndex(
                name: "IX_BalanceLog_BalanceId_Date",
                table: "BalanceLogs",
                newName: "IX_BalanceLogs_BalanceId_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceLogs",
                table: "BalanceLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceLogs_Balances_BalanceId",
                table: "BalanceLogs",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BalanceLogs_Balances_BalanceId",
                table: "BalanceLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BalanceLogs",
                table: "BalanceLogs");

            migrationBuilder.RenameTable(
                name: "BalanceLogs",
                newName: "BalanceLog");

            migrationBuilder.RenameIndex(
                name: "IX_BalanceLogs_BalanceId_Date",
                table: "BalanceLog",
                newName: "IX_BalanceLog_BalanceId_Date");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BalanceLog",
                table: "BalanceLog",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BalanceLog_Balances_BalanceId",
                table: "BalanceLog",
                column: "BalanceId",
                principalTable: "Balances",
                principalColumn: "Id");
        }
    }
}
