using LoanManagement.Entities;

namespace LoanManagement.Services.FinancialInformations.Contracts.DTOs
{
    public class AddFinancialInformationDto
    {
        public decimal MonthlyIncome { get; set; }
        public JobType Job { get; set; }
        public int CustomerId { get; set; }
        public decimal? FinancialAssets { get; set; }
    }
}
