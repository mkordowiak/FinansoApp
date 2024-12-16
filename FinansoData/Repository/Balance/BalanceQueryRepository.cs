using FinansoData.Data;
using FinansoData.DataViewModel.Balance;
using Microsoft.EntityFrameworkCore;

namespace FinansoData.Repository.Balance
{
    public class BalanceQueryRepository : IBalanceQueryRepository
    {
        private readonly ApplicationDbContext _context;

        public BalanceQueryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RepositoryResult<IEnumerable<BalanceViewModel>?>> GetListOfBalancesForGroup(int groupId)
        {
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

            try
            {
                List<BalanceViewModel> result = await query.ToListAsync();
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(result);
            }
            catch
            {
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Failure(null, ErrorType.ServerError);
            }
        }

        public async Task<RepositoryResult<IEnumerable<BalanceViewModel>?>> GetListOfBalancesForUser(string userName)
        {
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

            try
            {
                List<BalanceViewModel> result = await queryGroupMember.Union(queryGroupOwner).ToListAsync();
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Success(result);
            }
            catch
            {
                return RepositoryResult<IEnumerable<BalanceViewModel>>.Failure(null, ErrorType.ServerError);
            }
        }
    }
}
