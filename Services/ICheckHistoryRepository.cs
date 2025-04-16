using balcheckcalcweb.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace balcheckcalcweb.Services
{
    public interface ICheckHistoryRepository
    {
        Task<CheckHistory> SaveCheckHistoryAsync(CheckHistory checkHistory);
        Task<List<CheckHistory>> GetCheckHistoriesByAliasAsync(string userAlias);
        Task<CheckHistory> GetCheckHistoryByIdAsync(int id);
    }
}