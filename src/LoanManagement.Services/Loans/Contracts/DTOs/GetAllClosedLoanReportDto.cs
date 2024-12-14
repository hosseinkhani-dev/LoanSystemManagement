namespace LoanManagement.Services.Loans.Contracts.DTOs
{
    public class GetAllClosedLoanReportDto
    {
        public int LoanId { get; set; }
        public decimal Amount { get; set; }
        public byte RepaymentCount { get; set; }
        public decimal TotalLatePenalty { get; set; }
    }
}
