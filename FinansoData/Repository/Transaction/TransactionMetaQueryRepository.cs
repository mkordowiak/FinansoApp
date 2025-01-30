using FinansoData.Data;
using FinansoData.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Transaction
{
    public class TransactionMetaQueryRepository : ITransactionMetaQueryRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheWrapper _cacheWrapper;
        private readonly string _cacheClassName;

        public TransactionMetaQueryRepository(ApplicationDbContext applicationDbContext, ICacheWrapper cacheWrapper)
        {
            _applicationDbContext = applicationDbContext;
            _cacheWrapper = cacheWrapper;
            _cacheClassName = this.GetType().Name;
        }

        public async Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfAllTransactionStatuses()
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}";
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
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}";
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

        public async Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetTransactionIncomeCategories()
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<Tuple<int, string>> cacheData))
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(cacheData);
            }

            IQueryable<Tuple<int, string>> query = from tc in _applicationDbContext.TransactionCategories
                                                   where tc.TransactionTypeId.Equals(1)
                                                   select new Tuple<int, string>(tc.Id, tc.Name);

            List<Tuple<int, string>> result;
            try
            {
                result = await query.AsNoTracking().ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Failure("Error while getting transaction categories", ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromMinutes(30));
            return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(result);
        }


        public async Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetTransactionExpenseCategories()
        {
            string methodName = MethodName.GetMethodName();
            string cacheDataKey = $"{_cacheClassName}_{methodName}";
            if (_cacheWrapper.TryGetValue(cacheDataKey, out IEnumerable<Tuple<int, string>> cacheData))
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(cacheData);
            }
            IQueryable<Tuple<int, string>> query = from tc in _applicationDbContext.TransactionCategories
                                                   where tc.TransactionTypeId.Equals(2)
                                                   select new Tuple<int, string>(tc.Id, tc.Name);

            List<Tuple<int, string>> result;
            try
            {
                result = await query.AsNoTracking().ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Failure("Error while getting transaction categories", ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheDataKey, result, TimeSpan.FromMinutes(30));
            return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(result);
        }
    }
}
