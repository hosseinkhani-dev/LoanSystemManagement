using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.LoanTypes.Contracts.DTOs
{
    public class AddLoanTypeDto
    {
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal InterestRate { get; set; }
        [Required]
        public byte RepaymentPeriod { get; set; }
        [Required]
        public decimal MonthlyRepayment { get; set; }
    }
}
