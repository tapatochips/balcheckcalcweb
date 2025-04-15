using System;

namespace balcheckcalcweb.Services
{
    public interface IPolicyCalculatorService
    {
        decimal CalculateRevisedAmount(DateTime effectiveDate, DateTime expirationDate,
                                      DateTime currentDate, decimal balance, decimal installment);
        bool ValidateDateRange(DateTime effectiveDate, DateTime expirationDate, DateTime currentDate);
    }

    public class PolicyCalculatorService : IPolicyCalculatorService
    {
        public decimal CalculateRevisedAmount(DateTime effectiveDate, DateTime expirationDate,
                                             DateTime currentDate, decimal balance, decimal installment)
        {
            if (!ValidateDateRange(effectiveDate, expirationDate, currentDate))
            {
                throw new ArgumentException("Current date must be between effective and expiration dates.");
            }

            int monthsLeft = ((expirationDate.Year - currentDate.Year) * 12) +
                              (expirationDate.Month - currentDate.Month);

            return balance - (installment * monthsLeft);
        }

        public bool ValidateDateRange(DateTime effectiveDate, DateTime expirationDate, DateTime currentDate)
        {
            return currentDate >= effectiveDate && currentDate <= expirationDate;
        }
    }
}