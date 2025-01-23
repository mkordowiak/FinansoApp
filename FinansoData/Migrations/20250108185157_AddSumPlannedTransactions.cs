using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinansoData.Migrations
{
    /// <inheritdoc />
    public partial class AddSumPlannedTransactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE SumPlannedTransactions AS
BEGIN
	DECLARE @IdPlanned INT;
	DECLARE @IdCompleted INT;
	DECLARE @IdExpense INT;
	DECLARE @IdIncome INT;

	SELECT @IdPlanned = Id From TransactionStatuses WHERE [Name] = 'Planned'
	SELECT @IdCompleted = Id From TransactionStatuses WHERE [Name] = 'Completed'
	SELECT @IdExpense = Id From TransactionTypes WHERE [Name] = 'Expense'
	SELECT @IdIncome = Id From TransactionTypes WHERE [Name] = 'Income'

	DECLARE @transactionId INT;
	DECLARE @transactionAmount [decimal](18, 8);
	DECLARE @groupId INT;
	DECLARE @balanceId INT;
	DECLARE @balanceAmount [decimal](18,8)
	DECLARE @transactionTypeId INT;

	BEGIN TRANSACTION sum_transactions

	-- DECLARE CURSOR
	DECLARE cursor_transactions INSENSITIVE CURSOR 
	FOR 
	SELECT bt.Id, bt.Amount, bt.GroupId, bt.BalanceId, b.Amount, bt.TransactionTypeId
	FROM BalanceTransactions bt WITH (NOLOCK)
	JOIN Balances b WITH (NOLOCK) ON bt.BalanceId = b.Id 
	WHERE GETDATE() >  bt.TransactionDate 
	AND  bt.TransactionStatusId = @IdPlanned;

	-- OPEN CURSOR
	OPEN cursor_transactions;

	-- Fetch 
	FETCH NEXT FROM cursor_transactions INTO @transactionId, @transactionAmount, @groupId, @balanceId, @balanceAmount, @transactionTypeId

	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @transactionTypeId = @IdIncome 
		BEGIN
			UPDATE [Balances] SET [Amount] = (@balanceAmount + @transactionAmount) WHERE [Id] = @balanceId;
			UPDATE [BalanceTransactions] SET [TransactionStatusId] = @IdCompleted WHERE [Id] = @transactionId;
		END;

		IF @transactionTypeId = @IdExpense 
		BEGIN
			UPDATE [Balances] SET [Amount] = (@balanceAmount - @transactionAmount) WHERE [Id] = @balanceId;
			UPDATE [BalanceTransactions] SET [TransactionStatusId] = @IdCompleted WHERE [Id] = @transactionId;
		END;

		FETCH NEXT FROM cursor_transactions INTO @transactionId, @transactionAmount, @groupId, @balanceId, @balanceAmount, @transactionTypeId;
	END

	CLOSE cursor_transactions;
	DEALLOCATE cursor_transactions;

	COMMIT TRANSACTION sum_transactions;

END; ");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE SumPlannedTransactions");
        }
    }
}
