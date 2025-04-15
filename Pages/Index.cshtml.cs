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
        public string StatusMessage { get; set; }

        public void OnGet()
        {
            //pre-populate with a single policy on first load
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
                    //validate date ranges
                    if (!_calculatorService.ValidateDateRange(input.EffectiveDate, input.ExpirationDate, input.CurrentDate))
                    {
                        ModelState.AddModelError(string.Empty, 
                            $"Policy {input.PolicyNumber}: Current date must be between effective and expiration dates.");
                        continue;
                    }

                    //calc revised amount
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


        private decimal CalculateRevisedAmount(PolicyInputModel input)
        {
            if (!DateTime.TryParse(input.EffectiveDate, out var effectiveDate))
                throw new InvalidOperationException($"Policy {input.PolicyNumber}: Invalid Effective Date.");
            if (!DateTime.TryParse(input.ExpirationDate, out var expirationDate))
                throw new InvalidOperationException($"Policy {input.PolicyNumber}: Invalid Expiration Date.");
            if (!DateTime.TryParse(input.CurrentDate, out var currentDate))
                throw new InvalidOperationException($"Policy {input.PolicyNumber}: Invalid Current Date.");

            if (currentDate < effectiveDate || currentDate > expirationDate)
            {
                throw new InvalidOperationException($"Policy {input.PolicyNumber}: Date range out of bounds.");
            }

            int monthsLeft = ((expirationDate.Year - currentDate.Year) * 12) + (expirationDate.Month - currentDate.Month);
            return input.Balance - (input.Installment * monthsLeft);
        }
    

    public class PolicyInputModel
    {
        public int PolicyNumber { get; set; }
        public decimal Balance { get; set; }
        public decimal Installment { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpirationDate { get; set; }
        public string CurrentDate { get; set; }
    }

    public class PolicyResultModel
    {
        public int PolicyNumber { get; set; }
        public decimal RevisedAmount { get; set; }
    }
}
