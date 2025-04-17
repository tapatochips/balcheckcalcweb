using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using balcheckcalcweb.Services;
using balcheckcalcweb.Models;

namespace balcheckcalcweb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IPolicyCalculatorService _calculatorService;
        private readonly ICheckHistoryRepository _checkHistoryRepository;

        public IndexModel(IPolicyCalculatorService calculatorService, ICheckHistoryRepository checkHistoryRepository)
        {
            _calculatorService = calculatorService;
            _checkHistoryRepository = checkHistoryRepository;
        }

        [BindProperty]
        [Required(ErrorMessage = "User alias is required")]
        [StringLength(6, ErrorMessage = "Alias cannot exceed 6 characters")]
        [Display(Name = "Your Alias")]
        public string UserAlias { get; set; }

        [BindProperty]
        [Range(1, 100, ErrorMessage = "Please enter a value between 1 and 100")]
        public int PolicyCount { get; set; } = 1;

        [BindProperty]
        public List<PolicyInputModel> PolicyInputs { get; set; } = new List<PolicyInputModel>();

        public List<PolicyResultModel> Results { get; private set; } = new List<PolicyResultModel>();

        public decimal TotalAmount { get; private set; }

        [TempData]
        public string? StatusMessage { get; set; }

        //store the ID of the created check history for PDF generation
        [TempData]
        public int? SavedCheckHistoryId { get; set; }

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

        public async Task<IActionResult> OnPostCalculateAsync()
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
                return Page();
            }

            //save calc results to the database
            var checkHistory = new CheckHistory
            {
                UserAlias = UserAlias,
                PolicyCount = Results.Count,
                TotalAmount = TotalAmount,
                CalculationDate = DateTime.Now
            };

            //add policy details
            for (int i = 0; i < Results.Count; i++)
            {
                var policy = PolicyInputs[i];
                var result = Results[i];

                checkHistory.PolicyDetails.Add(new CheckHistoryDetail
                {
                    PolicyNumber = policy.PolicyNumber,
                    Balance = policy.Balance,
                    Installment = policy.Installment,
                    EffectiveDate = policy.EffectiveDate,
                    ExpirationDate = policy.ExpirationDate,
                    CurrentDate = policy.CurrentDate,
                    RevisedAmount = result.RevisedAmount
                });
            }

            try
            {
                var savedHistory = await _checkHistoryRepository.SaveCheckHistoryAsync(checkHistory);
                SavedCheckHistoryId = savedHistory.Id;
                StatusMessage = "Calculation saved successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error saving calculation: {ex.Message}";
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