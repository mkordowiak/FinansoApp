using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FinansoData.Data
{
    public class TimedActions : ITimedActions
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TimedActions> _logger;

        public TimedActions(ApplicationDbContext context, ILogger<TimedActions> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task UpdateBalanceTransactions()
        {
            _logger.LogInformation($"Updating balance transactions started at: {DateTime.Now}.");
            await _context.Database.ExecuteSqlRawAsync("EXEC [UpdateBalanceTransactions]");
            _logger.LogInformation($"Updating balance transactions finished at: {DateTime.Now}.");
        }
    }
}
