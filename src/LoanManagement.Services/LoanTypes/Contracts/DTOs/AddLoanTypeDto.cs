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
        public byte RepaymentPeriod { get; set; }
        [Required]
        public decimal MonthlyRepayment { get; set; }

        public decimal InterestRate
        {
            get
            {
                if (RepaymentPeriod > 11)
                {
                    return 0.2m;
                }
                else if (RepaymentPeriod < 12)
                {
                    return 0.15m;
                }
                return 0; 
            }
        }
    }
}
