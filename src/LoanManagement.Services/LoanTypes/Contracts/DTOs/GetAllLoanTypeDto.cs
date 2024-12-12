using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.LoanTypes.Contracts.DTOs
{
    public class GetAllLoanTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public decimal InterestRate { get; set; }
        public byte RepaymentPeriod { get; set; }
        public decimal MonthlyRepayment { get; set; }
    }
}
