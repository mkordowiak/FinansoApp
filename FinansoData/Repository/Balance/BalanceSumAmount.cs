using FinansoData.Data;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Balance
{
    public class BalanceSumAmount : IBalanceSumAmount
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;

        public BalanceSumAmount(ApplicationDbContext context, ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
        }


        public async Task<RepositoryResult<double?>> GetGroupBalancesAmount(int groupId)
        {
            string cacheKey = $"BalanceSumAmount_GetGroupBalancesAmount_{groupId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out double? cacheSum))
            {
                return RepositoryResult<double?>.Success(cacheSum);
            }

            var query = from b in _context.Balances
                        join c in _context.Currencies on b.Currency.Id equals c.Id
                        where b.Group.Id == groupId
                        group new { b, c } by b.GroupId into g
                        select new
                        {
                            GroupId = g.Key,
                            Sum = g.Sum(x => x.b.Amount * x.c.ExchangeRate)
                        };

            double sum;
            try
            {
                var result = await query.FirstOrDefaultAsync();
                sum = result.Sum;
            }
            catch
            {
                return RepositoryResult<double?>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, sum, TimeSpan.FromSeconds(60));
            return RepositoryResult<double?>.Success(sum);
        }

        public async Task<RepositoryResult<double?>> GetBalancesSumAmountForUser(string userName)
        {
            string cacheKey = $"BalanceSumAmount_GetBalancesSumAmountForUser_{userName}";
            if (_cacheWrapper.TryGetValue(cacheKey, out double? cachedSum))
            {
                return RepositoryResult<double?>.Success(cachedSum);
            }

            var queryGroupsOwnedByUser = from g in _context.Groups
                                         join u in _context.AppUsers on g.OwnerAppUser.Id equals u.Id
                                         join b in _context.Balances on g.Id equals b.GroupId
                                         join c in _context.Currencies on b.CurrencyId equals c.Id
                                         where u.NormalizedUserName == userName
                                         select new
                                         {
                                             g.Name,
                                             b.Amount,
                                             c.ExchangeRate,
                                             AmountNorm = b.Amount * c.ExchangeRate
                                         };

            var queryGroupsMember = from u in _context.AppUsers
                                    join gu in _context.GroupUsers on u.Id equals gu.AppUserId
                                    join g in _context.Groups on gu.GroupId equals g.Id
                                    join b in _context.Balances on g.Id equals b.GroupId
                                    join c in _context.Currencies on b.CurrencyId equals c.Id
                                    where u.NormalizedUserName == userName
                                    select new
                                    {
                                        g.Name,
                                        b.Amount,
                                        c.ExchangeRate,
                                        AmountNorm = b.Amount * c.ExchangeRate
                                    };

            var unionQuery = queryGroupsMember.Union(queryGroupsOwnedByUser);
            double sumAmountOfAllBalancesForUser;
            try
            {

                sumAmountOfAllBalancesForUser = await unionQuery.SumAsync(x => x.AmountNorm);

            }
            catch
            {
                return RepositoryResult<double?>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, sumAmountOfAllBalancesForUser, TimeSpan.FromSeconds(60));
            return RepositoryResult<double?>.Success(sumAmountOfAllBalancesForUser);
        }
    }
}
