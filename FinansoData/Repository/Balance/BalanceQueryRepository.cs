﻿using FinansoData.Data;
using FinansoData.DataViewModel.Balance;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Balance
{
    public class BalanceQueryRepository : IBalanceQueryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheWrapper _cacheWrapper;

        public BalanceQueryRepository(ApplicationDbContext context, ICacheWrapper cacheWrapper)
        {
            _context = context;
            _cacheWrapper = cacheWrapper;
        }

        public async Task<RepositoryResult<IEnumerable<BalanceViewModel>?>> GetListOfBalancesForGroup(int groupId)
        {
            string cacheKey = $"BalanceQueryRepository_GetListOfBalancesForGroup_{groupId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out List<BalanceViewModel> cachedBalanceVM))
            {
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(cachedBalanceVM);
            }


            IQueryable<BalanceViewModel> query = from g in _context.Groups
                                                 join b in _context.Balances on g.Id equals b.Group.Id
                                                 where g.Id == groupId
                                                 select new BalanceViewModel
                                                 {
                                                     Id = b.Id,
                                                     Name = b.Name,
                                                     Amount = b.Amount,
                                                     Currency = b.Currency,
                                                     Group = b.Group
                                                 };
            List<BalanceViewModel> result;
            try
            {
                result = await query.ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, result, TimeSpan.FromSeconds(30));
            return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(result);
        }

        public async Task<RepositoryResult<IEnumerable<BalanceViewModel>?>> GetListOfBalancesForUser(string userName)
        {
            string cacheKey = $"BalanceQueryRepository_GetListOfBalancesForUser_{userName}";
            if (_cacheWrapper.TryGetValue(cacheKey, out List<BalanceViewModel> cachedBalanceVM))
            {
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(cachedBalanceVM);
            }


            IQueryable<BalanceViewModel> queryGroupMember = from u in _context.Users
                                                            join gu in _context.GroupUsers on u.Id equals gu.AppUserId
                                                            join g in _context.Groups on gu.Group.Id equals g.Id
                                                            join b in _context.Balances on g.Id equals b.Group.Id
                                                            where u.UserName == userName
                                                            select new BalanceViewModel
                                                            {
                                                                Id = b.Id,
                                                                Name = b.Name,
                                                                Amount = b.Amount,
                                                                Currency = b.Currency,
                                                                Group = b.Group
                                                            };

            IQueryable<BalanceViewModel> queryGroupOwner = from u in _context.Users
                                                           join g in _context.Groups on u.Id equals g.OwnerAppUser.Id
                                                           join b in _context.Balances on g.Id equals b.Group.Id
                                                           where u.UserName == userName
                                                           select new BalanceViewModel
                                                           {
                                                               Id = b.Id,
                                                               Name = b.Name,
                                                               Amount = b.Amount,
                                                               Currency = b.Currency,
                                                               Group = b.Group
                                                           };
            List<BalanceViewModel> result;
            try
            {
                result = await queryGroupMember.Union(queryGroupOwner).ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, result, TimeSpan.FromSeconds(3));
            return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(result);
        }

        public async Task<RepositoryResult<bool?>> HasUserAccessToBalance(string userName, int balanceId)
        {
            string cacheKey = $"BalanceQueryRepository_HasUserAccessToBalance_{userName}_{balanceId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out bool cachedResult))
            {
                return RepositoryResult<bool?>.Success(cachedResult);
            }

            IQueryable<bool> queryGroupOwner = from u in _context.Users
                                               join g in _context.Groups on u.Id equals g.OwnerAppUser.Id
                                               join b in _context.Balances on g.Id equals b.Group.Id
                                               where u.NormalizedUserName == userName
                                                && b.Id == balanceId
                                               select true;

            IQueryable<bool> queryGroupMember = from u in _context.Users
                                                join gu in _context.GroupUsers on u.Id equals gu.AppUserId
                                                join g in _context.Groups on gu.Group.Id equals g.Id
                                                join b in _context.Balances on g.Id equals b.Group.Id
                                                where u.NormalizedUserName == userName
                                                    && b.Id == balanceId
                                                    && gu.Active == true
                                                select true;
            bool result;
            try
            {
                result = await queryGroupOwner.Union(queryGroupMember).AnyAsync();
            }
            catch
            {
                return RepositoryResult<bool?>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, result, TimeSpan.FromSeconds(30));
            return RepositoryResult<bool?>.Success(result);
        }


        public async Task<RepositoryResult<BalanceViewModel>> GetBalance(int balcnceId)
        {
            string cacheKey = $"BalanceQueryRepository_GetBalance_{balcnceId}";
            if (_cacheWrapper.TryGetValue(cacheKey, out BalanceViewModel cachedBalanceVM))
            {
                return RepositoryResult<BalanceViewModel>.Success(cachedBalanceVM);
            }

            IQueryable<BalanceViewModel> query = from b in _context.Balances
                                                 where b.Id == balcnceId
                                                 select new BalanceViewModel
                                                 {
                                                     Id = b.Id,
                                                     Name = b.Name,
                                                     Amount = b.Amount,
                                                     Currency = b.Currency,
                                                     Group = b.Group
                                                 };
            BalanceViewModel result;
            try
            {
                result = await query.SingleOrDefaultAsync();
            }
            catch
            {
                return RepositoryResult<BalanceViewModel>.Failure(null, ErrorType.ServerError);
            }


            if (result == null)
            {
                return RepositoryResult<BalanceViewModel>.Failure(null, ErrorType.NotFound);
            }

            _cacheWrapper.Set(cacheKey, result, TimeSpan.FromSeconds(30));
            return RepositoryResult<BalanceViewModel>.Success(result);
        }

        public async Task<RepositoryResult<double?>> GetBalancesSumAmountForUser(string userName)
        {
            string cacheKey = $"BalanceQueryRepository_GetBalancesSumAmountForUser_{userName}";
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
