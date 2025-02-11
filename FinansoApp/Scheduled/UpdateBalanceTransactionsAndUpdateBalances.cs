using FinansoData.Data;
using FinansoData.Repository.Procedures;
using FinansoData.Repository.Settings;
using Quartz;

namespace FinansoApp.Scheduled
{
    public class UpdateBalanceTransactionsAndUpdateBalances : IJob
    {
        private readonly ISumBalanceProcedure _sumBalanceProcedure;
        private readonly ISettingsQueryRepository _settingsQueryRepository;

        public UpdateBalanceTransactionsAndUpdateBalances(ISumBalanceProcedure sumBalanceProcedure, ISettingsQueryRepository settingsQueryRepository)
        {
            _sumBalanceProcedure = sumBalanceProcedure;
            _settingsQueryRepository = settingsQueryRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            int numOfRowsToBeProceeded = await _settingsQueryRepository.GetSettingsAsync<int>("BalanceTransactionsNumberOfRowsToBeProceeded");
            await _sumBalanceProcedure.UpdateBalancesAndTransactions(numOfRowsToBeProceeded);
        }
    }
}
