using LoanManagement.Entities;

namespace LoanManagement.Services.Customers.Contracts.DTOs
{
    public class EditFinancialInformationDto
    {
        public decimal MonthlyIncome { get; set; }
        public JobType Job { get; set; }
        public decimal? FinancialAssets { get; set; }
    }
}
