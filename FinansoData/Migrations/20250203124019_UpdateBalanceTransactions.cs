using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinansoData.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBalanceTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE [dbo].[UpdateBalanceTransactions] @NumOfRowsToProcedure INT
AS 
/*
-- =============================================
-- Author:      MJK
-- Create Date: 03.02.2025
-- Description: Updates balance transactions and connected balances
-- =============================================
*/
BEGIN
	DECLARE @transactionPlannedId INT;
	DECLARE @transactionCompletedId INT;
	DECLARE @incomeTypeId INT;
	DECLARE @expenseStatusId INT;
	DECLARE @transactionId INT;
	DECLARE @transactionAmount [DECIMAL](18, 8);
	DECLARE @balanceAmount [DECIMAL](18,8);
	DECLARE @transactionStatusId INT;
	DECLARE @balanceId INT;
	DECLARE @transactionTypeId INT;

	SELECT @transactionPlannedId = [Id] FROM [dbo].[TransactionStatuses] WHERE [Name] = 'Planned';
	SELECT @transactionCompletedId = [Id] FROM [dbo].[TransactionStatuses] WHERE [Name] = 'Completed';
	SELECT @incomeTypeId = [Id] FROM [TransactionTypes] WHERE [Name] = 'Income';
	SELECT @expenseStatusId = [Id] FROM [TransactionTypes] WHERE [Name] = 'Expense';

	DECLARE TransactionCursor CURSOR LOCAL FAST_FORWARD FOR
		SELECT TOP (@NumOfRowsToProcedure) [Id], [BalanceId], [Amount], [TransactionStatusId], [TransactionTypeId] 
		FROM [dbo].[BalanceTransactions]
		WHERE [TransactionStatusId] = @transactionPlannedId
			AND [TransactionDate] < GETDATE();

	OPEN TransactionCursor;

	FETCH NEXT FROM TransactionCursor INTO @transactionId, @balanceId, @transactionAmount, @transactionStatusId, @transactionTypeId;

	WHILE @@FETCH_STATUS = 0
	BEGIN
		UPDATE [dbo].[BalanceTransactions]
		SET [TransactionStatusId] = @transactionCompletedId, [ModifiedAt] = GETDATE()
		WHERE [Id] = @transactionId;

		SELECT @balanceAmount = [Amount] FROM [dbo].[Balances] WHERE [Id] = @balanceId;

		IF @transactionTypeId = @incomeTypeId 
		BEGIN
			PRINT N'Income added to balance';
			SET @balanceAmount = @balanceAmount + @transactionAmount;
		END;

		IF @transactionTypeId = @expenseStatusId
		BEGIN
			PRINT N'Expense added to balance';
			SET @balanceAmount = @balanceAmount - @transactionAmount;
		END;

		UPDATE [Balances]
		SET [Amount] = @balanceAmount,
		[Modified] = GETDATE()
		WHERE [Id] = @balanceId;

		FETCH NEXT FROM TransactionCursor INTO @transactionId, @balanceId, @transactionAmount, @transactionStatusId, @transactionTypeId;

	END

	CLOSE TransactionCursor;
	DEALLOCATE TransactionCursor;

END
GO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE [UpdateBalanceTransactions]");
        }
    }
}
