namespace LoanManagement.Services.Loans.Contracts.DTOs
{
    public class GetAllLoanActiveReportDto
    {
        public int LoanId { get; set; }
        public string LoanState { get; set; }
        public decimal TotalRepaid { get; set; }
        public int RemainingRepayments { get; set; }
    }
}
