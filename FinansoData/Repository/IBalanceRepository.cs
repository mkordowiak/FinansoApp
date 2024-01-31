using FinansoData.Models;
using static FinansoData.Repository.IBalanceRepository;

namespace FinansoData.Repository
{
    public interface IBalanceRepository
    {
        IBalanceRepositoryErrorInfo Error { get; }

        public interface IBalanceRepositoryErrorInfo
        {
            bool DatabaseError { get; set; }
        }


        bool Add(Balance balance);
        bool Update(Balance balance);
        bool Delete(Balance balance);
        bool Save();
        Task<bool> SaveAsync();
        Task<bool> UpdateAsync(Balance balance);

        Task<Balance?> GetBalanceAsync(int id);
        Task<IEnumerable<Balance>?> GetAllBalancesAsync();

    }


    public class BalanceRepositoryErrorInfo : IBalanceRepositoryErrorInfo
    {
        public bool DatabaseError { get; set; }
    }
}
