using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinansoData.Migrations
{
    /// <inheritdoc />
    public partial class balancetrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TRIGGER [dbo].[trg_AfterUpdate_Balances]
ON [dbo].[Balances]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Insert the results from the INSERTED and DELETED tables into the BalanceLog table
    INSERT INTO [dbo].[BalanceLogs] ([Amount], [Date], [BalanceId])
    SELECT 
        i.[Amount], 
        GETDATE(), 
        i.[Id]
    FROM 
        INSERTED i
    INNER JOIN 
        DELETED d ON i.Id = d.Id
    WHERE 
        i.[Amount] <> d.[Amount];
END;
GO

ALTER TABLE [dbo].[Balances] ENABLE TRIGGER [trg_AfterUpdate_Balances]
GO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP TRIGGER [dbo].[trg_AfterUpdate_Balances]");
        }
    }
}
