using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Linq;

namespace balcheckcalcweb.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public int PolicyCount { get; set; }

        [BindProperty]
        public List<PolicyInputModel> PolicyInputs { get; set; } = new List<PolicyInputModel>();

        public List<PolicyResultModel> Results { get; private set; } = new List<PolicyResultModel>();

        public decimal TotalAmount { get; private set; }

        public void OnPostGenerateFields()
        {
            //gen input fields based on PolicyCount
            if (PolicyCount > 0)
            {
                PolicyInputs = Enumerable.Range(1, PolicyCount)
                                         .Select(i => new PolicyInputModel { PolicyNumber = i })
                                         .ToList();
            }
        }

        public IActionResult OnPostCalculate()
        {
            //check inputs are valid
            if (PolicyInputs == null || !PolicyInputs.Any())
            {
                ModelState.AddModelError(string.Empty, "Please provide valid policy details.");
                return Page();
            }

            Results.Clear();
            TotalAmount = 0;

            foreach (var input in PolicyInputs)
            {
                try
                {
                    var revisedAmount = CalculateRevisedAmount(input);
                    Results.Add(new PolicyResultModel
                    {
                        PolicyNumber = input.PolicyNumber + 1,
                        RevisedAmount = revisedAmount
                    });

                    TotalAmount += revisedAmount;
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return Page();
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
