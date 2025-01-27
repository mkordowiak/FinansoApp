using FinansoData.Data;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Transaction
{
    public class TransactionMetaQueryRepository : ITransactionMetaQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheWrapper _cacheWrapper;

        public TransactionMetaQueryRepository(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _applicationDbContext = applicationDbContext;
            _cacheWrapper = cacheWrapper;
        }

        public async Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfAllTransactionStatuses()
        {
            string cacheDataKey = $"TransactionMetaQueryRepository_GetShortListOfAllTransactionStatuses";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<Tuple<int, string>>? cacheTransactionTypes))
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(cacheTransactionTypes);
            }

            IQueryable<Tuple<int, string>> query = from transactionStatuses in _applicationDbContext.TransactionStatuses
                                                   select new Tuple<int, string>(transactionStatuses.Id, transactionStatuses.Name);

            List<Tuple<int, string>> result;
            try
            {
                result = await query.AsNoTracking().ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Failure("Error while getting transaction types", ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromMinutes(30));
            return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(result);
        }

        public async Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfAllTransactionTypes()
        {
            string cacheDataKey = $"TransactionMetaQueryRepository_GetShortListOfAllTransactionTypes";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<Tuple<int, string>>? cacheTransactionTypes))
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(cacheTransactionTypes);
            }

            IQueryable<Tuple<int, string>> query = from transactionType in _applicationDbContext.TransactionTypes
                                                   select new Tuple<int, string>(transactionType.Id, transactionType.Name);

            List<Tuple<int, string>> result;
            try
            {
                result = await query.AsNoTracking().ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Failure("Error while getting transaction types", ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromMinutes(30));
            return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(result);
        }
    }
}
