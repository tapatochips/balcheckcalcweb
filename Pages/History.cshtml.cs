using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using balcheckcalcweb.Services;
using balcheckcalcweb.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace balcheckcalcweb.Pages
{
    public class HistoryModel : PageModel
    {
        private readonly ICheckHistoryRepository _checkHistoryRepository;

        public HistoryModel(ICheckHistoryRepository checkHistoryRepository)
        {
            _checkHistoryRepository = checkHistoryRepository;
        }

        [BindProperty]
        [Required(ErrorMessage = "User alias is required for search")]
        public string SearchAlias { get; set; }

        public List<CheckHistory> CheckHistories { get; private set; } = new List<CheckHistory>();

        public CheckHistory DetailedHistory { get; private set; }

        [TempData]
        public string? StatusMessage { get; set; }

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                DetailedHistory = await _checkHistoryRepository.GetCheckHistoryByIdAsync(id.Value);
                if (DetailedHistory != null)
                {
                    SearchAlias = DetailedHistory.UserAlias;
                    CheckHistories = await _checkHistoryRepository.GetCheckHistoriesByAliasAsync(SearchAlias);
                }
                else
                {
                    StatusMessage = "Record not found.";
                }
            }
        }

        public async Task<IActionResult> OnPostSearchAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            CheckHistories = await _checkHistoryRepository.GetCheckHistoriesByAliasAsync(SearchAlias);

            if (!CheckHistories.Any())
            {
                StatusMessage = $"No calculations found for alias '{SearchAlias}'.";
            }

            return Page();
        }

        public async Task<IActionResult> OnGetDetailsAsync(int id)
        {
            DetailedHistory = await _checkHistoryRepository.GetCheckHistoryByIdAsync(id);

            if (DetailedHistory == null)
            {
                StatusMessage = "Record not found.";
                return RedirectToPage();
            }

            SearchAlias = DetailedHistory.UserAlias;
            CheckHistories = await _checkHistoryRepository.GetCheckHistoriesByAliasAsync(SearchAlias);

            return Page();
        }
    }
}