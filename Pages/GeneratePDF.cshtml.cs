using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using balcheckcalcweb.Services;
using System.Threading.Tasks;

namespace balcheckcalcweb.Pages
{
    public class GeneratePDFModel : PageModel
    {
        private readonly ICheckHistoryRepository _checkHistoryRepository;
        private readonly IPdfService _pdfService;

        public GeneratePDFModel(ICheckHistoryRepository checkHistoryRepository, IPdfService pdfService)
        {
            _checkHistoryRepository = checkHistoryRepository;
            _pdfService = pdfService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var checkHistory = await _checkHistoryRepository.GetCheckHistoryByIdAsync(id);

            if (checkHistory == null)
            {
                return NotFound();
            }

            var pdfBytes = _pdfService.GenerateCalculationPdf(checkHistory);

            return File(
                pdfBytes,
                "application/pdf",
                $"Policy_Calculation_{checkHistory.UserAlias}_{checkHistory.CalculationDate:yyyyMMdd}.pdf");
        }
    }
}