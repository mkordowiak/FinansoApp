using FinansoData.Data;
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


            IQueryable<BalanceViewModel> query = from g in _context.Groups.AsNoTracking()
                                                 join b in _context.Balances.AsNoTracking() on g.Id equals b.Group.Id
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
            IQueryable<BalanceViewModel> queryGroupMember = from u in _context.Users.AsNoTracking()
                                                            join gu in _context.GroupUsers.AsNoTracking() on u.Id equals gu.AppUserId
                                                            join g in _context.Groups.AsNoTracking() on gu.Group.Id equals g.Id
                                                            join b in _context.Balances.AsNoTracking() on g.Id equals b.Group.Id
                                                            where u.UserName == userName
                                                            select new BalanceViewModel
                                                            {
                                                                Id = b.Id,
                                                                Name = b.Name,
                                                                Amount = b.Amount,
                                                                Currency = b.Currency,
                                                                Group = b.Group
                                                            };

            IQueryable<BalanceViewModel> queryGroupOwner = from u in _context.Users.AsNoTracking()
                                                           join g in _context.Groups.AsNoTracking() on u.Id equals g.OwnerAppUser.Id
                                                           join b in _context.Balances.AsNoTracking() on g.Id equals b.Group.Id
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

            IQueryable<bool> queryGroupOwner = from u in _context.Users.AsNoTracking()
                                               join g in _context.Groups.AsNoTracking() on u.Id equals g.OwnerAppUser.Id
                                               join b in _context.Balances.AsNoTracking() on g.Id equals b.Group.Id
                                               where u.NormalizedUserName == userName
                                                && b.Id == balanceId
                                               select true;

            IQueryable<bool> queryGroupMember = from u in _context.Users.AsNoTracking()
                                                join gu in _context.GroupUsers.AsNoTracking() on u.Id equals gu.AppUserId
                                                join g in _context.Groups.AsNoTracking() on gu.Group.Id equals g.Id
                                                join b in _context.Balances.AsNoTracking() on g.Id equals b.Group.Id
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

            IQueryable<BalanceViewModel> query = from b in _context.Balances.AsNoTracking()
                                                 where b.Id == balcnceId
                                                 select new BalanceViewModel
                                                 {
                                                     Id = b.Id,
                                                     Name = b.Name,
                                                     Amount = b.Amount,
                                                     Currency = b.Currency,
                                                     Group = b.Group
                                                 };
            BalanceViewModel? result;
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


        public async Task<RepositoryResult<IEnumerable<Tuple<int, string>>>> GetShortListOfBalanceForUser(string userName)
        {
            string cacheKey = $"BalanceQueryRepository_GetShortListOfBalanceForUser_{userName}";
            if (_cacheWrapper.TryGetValue(cacheKey, out List<Tuple<int, string>> cachedBalanceVM))
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(cachedBalanceVM);
            }
            IQueryable<Models.Balance> queryGroupMember = from u in _context.Users.AsNoTracking()
                                                          join gu in _context.GroupUsers.AsNoTracking() on u.Id equals gu.AppUserId
                                                          join g in _context.Groups.AsNoTracking() on gu.Group.Id equals g.Id
                                                          join b in _context.Balances.AsNoTracking() on g.Id equals b.Group.Id
                                                          where u.UserName == userName
                                                          select b;

            IQueryable<Models.Balance> queryGroupOwner = from u in _context.Users.AsNoTracking()
                                                         join g in _context.Groups.AsNoTracking() on u.Id equals g.OwnerAppUser.Id
                                                         join b in _context.Balances.AsNoTracking() on g.Id equals b.Group.Id
                                                         where u.UserName == userName
                                                         select b;
            List<Tuple<int, string>> result;
            try
            {
                result = await queryGroupMember.Union(queryGroupOwner)
                                       .Select(b => new Tuple<int, string>(b.Id, b.Name))
                                       .ToListAsync();
            }
            catch
            {
                return RepositoryResult<IEnumerable<Tuple<int, string>>>.Failure(null, ErrorType.ServerError);
            }

            _cacheWrapper.Set(cacheKey, result, TimeSpan.FromSeconds(3));
            return RepositoryResult<IEnumerable<Tuple<int, string>>>.Success(result);
        }
    }
}
