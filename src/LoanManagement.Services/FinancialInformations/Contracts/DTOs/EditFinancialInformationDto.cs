using LoanManagement.Entities;

namespace LoanManagement.Services.FinancialInformations.Contracts.DTOs
{
    public class EditFinancialInformationDto
    {
        public decimal MonthlyIncome { get; set; }
        public JobType Job { get; set; }
        public decimal? FinancialAssets { get; set; }
    }
}
