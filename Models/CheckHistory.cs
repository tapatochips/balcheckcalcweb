using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace balcheckcalcweb.Models
{
    public class CheckHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserAlias { get; set; }

        [Required]
        public DateTime CalculationDate { get; set; } = DateTime.Now;

        public int PolicyCount { get; set; }

        public decimal TotalAmount { get; set; }

        // Navigation property
        public List<CheckHistoryDetail> PolicyDetails { get; set; } = new List<CheckHistoryDetail>();
    }

    public class CheckHistoryDetail
    {
        [Key]
        public int Id { get; set; }

        public int CheckHistoryId { get; set; }

        public int PolicyNumber { get; set; }

        public decimal Balance { get; set; }

        public decimal Installment { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public DateTime CurrentDate { get; set; }

        public decimal RevisedAmount { get; set; }

        // Navigation property
        public CheckHistory CheckHistory { get; set; }
    }
}