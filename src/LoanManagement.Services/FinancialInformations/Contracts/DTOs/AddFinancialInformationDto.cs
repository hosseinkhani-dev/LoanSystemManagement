using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.FinancialInformations.Contracts.DTOs
{
    public class AddFinancialInformationDto
    {
        public decimal MonthlyIncome { get; set; }
        [StringLength(20)]
        public string Job { get; set; }
        public int CustomerId { get; set; }
        public decimal? FinancialAssets { get; set; }
    }
}
