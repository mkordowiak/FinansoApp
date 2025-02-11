namespace FinansoData.Repository.Procedures
{
    public interface ISumBalanceProcedure
    {
        /// <summary>
        /// Run [UpdateBalanceTransactions] SQL procedure
        /// </summary>
        /// <param name="numOfRowsToBeProceeded"></param>
        /// <returns></returns>
        Task<RepositoryResult<bool>> UpdateBalancesAndTransactions(int NumberOfRowsToBeProceeded);
    }
}
