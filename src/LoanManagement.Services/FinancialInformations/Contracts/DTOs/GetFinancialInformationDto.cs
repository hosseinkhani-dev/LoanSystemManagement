namespace LoanManagement.Services.FinancialInformations.Contracts.DTOs
{
    public class GetFinancialInformationDto
    {
        public int Id { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string Job { get; set; }
        public decimal? FinancialAssets { get; set; }
    }
}
