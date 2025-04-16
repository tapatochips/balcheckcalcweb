using balcheckcalcweb.Data;
using balcheckcalcweb.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace balcheckcalcweb.Services
{
    public class CheckHistoryRepository : ICheckHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CheckHistoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CheckHistory> SaveCheckHistoryAsync(CheckHistory checkHistory)
        {
            _context.CheckHistories.Add(checkHistory);
            await _context.SaveChangesAsync();
            return checkHistory;
        }

        public async Task<List<CheckHistory>> GetCheckHistoriesByAliasAsync(string userAlias)
        {
            return await _context.CheckHistories
                .Where(h => h.UserAlias.ToLower() == userAlias.ToLower())
                .OrderByDescending(h => h.CalculationDate)
                .Include(h => h.PolicyDetails)
                .ToListAsync();
        }

        public async Task<CheckHistory> GetCheckHistoryByIdAsync(int id)
        {
            return await _context.CheckHistories
                .Include(h => h.PolicyDetails)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
    }
}