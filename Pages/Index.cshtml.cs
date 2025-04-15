using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using balcheckcalcweb.Services;

namespace balcheckcalcweb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPolicyCalculatorService _calculatorService;

        public IndexModel(IPolicyCalculatorService calculatorService)
        {
            _calculatorService = calculatorService;
        }

        [BindProperty]
        [Range(1, 100, ErrorMessage = "Please enter a value between 1 and 100")]
        public int PolicyCount { get; set; } = 1;

        [BindProperty]
        public List<PolicyInputModel> PolicyInputs { get; set; } = new List<PolicyInputModel>();

        public List<PolicyResultModel> Results { get; private set; } = new List<PolicyResultModel>();

        public decimal TotalAmount { get; private set; }

        [TempData]
        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            // Pre-populate with a single policy on first load
            if (!PolicyInputs.Any())
            {
                PolicyCount = 1;
                PolicyInputs = new List<PolicyInputModel> { new PolicyInputModel { PolicyNumber = 1 } };
            }
        }

        public void OnPostGenerateFields()
        {
            if (PolicyCount > 0)
            {
                PolicyInputs = Enumerable.Range(1, PolicyCount)
                                        .Select(i => new PolicyInputModel { PolicyNumber = i })
                                        .ToList();
            }
            else
            {
                ModelState.AddModelError(nameof(PolicyCount), "Please enter at least 1 policy.");
            }
        }

        public IActionResult OnPostCalculate()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Results.Clear();
            TotalAmount = 0;

            foreach (var input in PolicyInputs)
            {
                try
                {
                    // Validate date ranges
                    if (!_calculatorService.ValidateDateRange(input.EffectiveDate, input.ExpirationDate, input.CurrentDate))
                    {
                        ModelState.AddModelError(string.Empty,
                            $"Policy {input.PolicyNumber}: Current date must be between effective and expiration dates.");
                        continue;
                    }

                    // Calculate revised amount
                    decimal revisedAmount = _calculatorService.CalculateRevisedAmount(
                        input.EffectiveDate,
                        input.ExpirationDate,
                        input.CurrentDate,
                        input.Balance,
                        input.Installment);

                    Results.Add(new PolicyResultModel
                    {
                        PolicyNumber = input.PolicyNumber,
                        RevisedAmount = revisedAmount
                    });

                    TotalAmount += revisedAmount;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Policy {input.PolicyNumber}: {ex.Message}");
                }
            }

            if (!Results.Any())
            {
                StatusMessage = "No valid policies to calculate.";
            }

            return Page();
        }
    }

    public class PolicyInputModel
    {
        public int PolicyNumber { get; set; }

        [Required(ErrorMessage = "Balance is required")]
        [Display(Name = "Balance")]
        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        [Required(ErrorMessage = "Installment amount is required")]
        [Display(Name = "Installment")]
        [DataType(DataType.Currency)]
        public decimal Installment { get; set; }

        [Required(ErrorMessage = "Effective date is required")]
        [Display(Name = "Effective Date")]
        [DataType(DataType.Date)]
        public DateTime EffectiveDate { get; set; }

        [Required(ErrorMessage = "Expiration date is required")]
        [Display(Name = "Expiration Date")]
        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }

        [Required(ErrorMessage = "Current date is required")]
        [Display(Name = "Current Date")]
        [DataType(DataType.Date)]
        public DateTime CurrentDate { get; set; }
    }

    public class PolicyResultModel
    {
        public int PolicyNumber { get; set; }
        public decimal RevisedAmount { get; set; }
    }
}