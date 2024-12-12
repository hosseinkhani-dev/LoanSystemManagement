using LoanManagement.Entities;
using System.ComponentModel.DataAnnotations;

namespace LoanManagement.Services.FinancialInformations.Contracts.DTOs
{
    public class GetFinancialInformationDto
    {
        public decimal MonthlyIncome { get; set; }
        public string Job { get; set; }
        public decimal? FinancialAssets { get; set; }
    }
}
