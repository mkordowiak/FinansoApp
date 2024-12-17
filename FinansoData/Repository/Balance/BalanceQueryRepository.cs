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
            if(_cacheWrapper.TryGetValue($"GetListOfBalancesForGroup_{groupId}", out List<BalanceViewModel> cachedBalanceVM))
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

            _cacheWrapper.Set($"GetListOfBalancesForGroup_{groupId}", result, TimeSpan.FromSeconds(30));
            return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(result);
        }

        public async Task<RepositoryResult<IEnumerable<BalanceViewModel>?>> GetListOfBalancesForUser(string userName)
        {
            if (_cacheWrapper.TryGetValue($"GetListOfBalancesForUser_{userName}", out List<BalanceViewModel> cachedBalanceVM))
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
                                                               //join gg in _context.Groups on u.Id equals gg.OwnerAppUser.Id
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

            _cacheWrapper.Set($"GetListOfBalancesForUser_{userName}", result, TimeSpan.FromSeconds(3));
            return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(result);
        }
    }
}
